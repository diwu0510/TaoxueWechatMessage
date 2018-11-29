using DotNetCore.CAP;
using HZC.Core;
using HZC.Database;
using HZC.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Taoxue.Mp.Sms.Services
{
    public class OMessageService
    {
        private readonly MyDbUtil db;
        private readonly ICapPublisher _publisher;

        public OMessageService(ICapPublisher publisher, string sectionName = "")
        {
            db = new MyDbUtil(sectionName);
            _publisher = publisher;
        }

        /// <summary>
        /// Api请求过来后，验证请求并记录LOG，若验证通过，发布OMessage.Create事件，供队列订阅调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Result Create(OMessageCreateDto entity, string ip, string method)
        {
            ApiLogEntity log = new ApiLogEntity
            {
                IP = ip,
                Method = method,
                PlatId = entity.PlatId,
                Group = entity.Group,
                ApiName = "OMessageCreate",
                Content = JsonConvert.SerializeObject(entity),
                CreateAt = DateTime.Now,
                IsOk = true,
                Message = "",
                Remark = ""
            };

            List<string> _errors = new List<string>();

            // 先简单的验证下参数
            if (entity.Content == null || entity.Content.Length == 0)
            {
                _errors.Add("消息内容不能为空");
            }

            if (entity.PlatId <= 0)
            {
                _errors.Add("PlatId必须大于0");
            }

            if (entity.TemplateId <= 0)
            {
                _errors.Add("TemplateId必须大于0");
            }

            if (entity.SendAt == null || entity.SendAt < DateTime.Today)
            {
                _errors.Add("消息发送时间不得小于当天日期");
            }

            // 验证链接地址
            if (!string.IsNullOrWhiteSpace(entity.Url))
            {
                if (!StringValidateUtil.IsUrl(entity.Url))
                {
                    _errors.Add("Url格式错误");
                }
            }

            // 验证类型
            if (entity.Type != 1 && entity.Type != 2)
            {
                _errors.Add("Type仅支持 1 Mobile | 2 OpenId");
            }

            if (_errors.Count > 0)
            {
                log.IsOk = false;
                log.Message = string.Join(";", _errors);
                log.Remark = "";

                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail(log.Message);
            }

            // 验证来源
            var plat = PlatUtil.Get(entity.PlatId);
            if (plat == null || !plat.Enabled)
            {
                log.IsOk = false;
                log.Message = "平台不存在或已禁用";
                db.Create<ApiLogEntity>(log);

                return ResultUtil.AuthFail("平台不存在或已禁用");
            }

            // 验证模板
            var temp = MessageTemplateUtil.Get(entity.TemplateId);
            if (temp == null || !temp.Enabled)
            {
                log.IsOk = false;
                log.Message = "模板不存在或已禁用";
                db.Create<ApiLogEntity>(log);

                return ResultUtil.AuthFail("模板不存在或已禁用");
            }

            // 清洗手机号码或OpenId
            List<string> _targets = new List<string>();

            if (entity.Type == 1)
            {
                foreach (var t in entity.Target)
                {
                    if (StringValidateUtil.IsMobile(t.Trim()))
                    {
                        _targets.Add(t.Trim());
                    }
                }
            }

            if (entity.Type == 2)
            {
                foreach (var t in entity.Target)
                {
                    if (StringValidateUtil.IsOpenId(t.Trim()))
                    {
                        _targets.Add(t.Trim());
                    }
                }
            }

            if (_targets.Count == 0)
            {
                log.IsOk = false;
                log.Message = "未包含有效发送对象";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("未包含有效发送对象");
            }

            // 验证发送数量
            if (_targets.Count > 1000)
            {
                log.IsOk = false;
                log.Message = "每次推送必须小于等于50条";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("每次推送必须小于等于50条");
            }
            
            // 写接口请求日志，添加到队列
            using (var conn = db.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        entity.Target = _targets.ToArray();
                        _publisher.Publish("Taoxue.Sms.OMessage.Create", entity);
                        trans.Commit();

                        return ResultUtil.Success();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ResultUtil.Exception(ex);
                    }
                }
            }
        }

        /// <summary>
        /// 把请求转换成原始消息
        /// </summary>
        /// <param name="dto"></param>
        public void OMessageCreate(OMessageCreateDto entity)
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var t in entity.Target)
                        {
                            var model = new OMessageEntity
                            {
                                Type = entity.Type,
                                Target = t,
                                Content = JsonConvert.SerializeObject(entity.Content),
                                Group = entity.Group,
                                PlatId = entity.PlatId,
                                TemplateId = entity.TemplateId,
                                Url = entity.Url,
                                SendAt = entity.SendAt,
                                ResultCode = (int)OMessageResultCode.Waiting,
                                ResultMessage = OMessageResultCode.Waiting.GetDescription(),
                                CreateAt = DateTime.Now
                            };

                            var id = db.Create<OMessageEntity>(model);
                            entity.Id = id;

                            _publisher.Publish("Taoxue.Sms.OMessage.Create", entity);
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        conn.Close();
                        throw ex;
                    }
                }
            }
        }
        
        /// <summary>
        /// 原始消息转换为通知消息
        /// </summary>
        /// <param name="dto"></param>
        public void OMessageOperate(OMessageQueueDto dto)
        {

        }
    }
}
