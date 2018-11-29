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

            // 先简单的验证下参数
            if (entity.Content == null || entity.Content.Length == 0)
            {
                log.IsOk = false;
                log.Message = "消息内容不能为空";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("消息内容不能为空");
            }

            if (entity.PlatId <= 0)
            {
                log.IsOk = false;
                log.Message = "不合法的PlatId";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("不合法的PlatId");
            }

            if (entity.TemplateId <= 0)
            {
                log.IsOk = false;
                log.Message = "不合法的TemplateId";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("不合法的TemplateId");
            }

            if (entity.SendAt == null || entity.SendAt < DateTime.Today)
            {
                log.IsOk = false;
                log.Message = "消息发送时间不得小于当天日期";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("消息发送时间不得小于当天日期");
            }

            // 验证链接地址
            if (!string.IsNullOrWhiteSpace(entity.Url))
            {
                if (!StringValidateUtil.IsUrl(entity.Url))
                {
                    log.IsOk = false;
                    log.Message = "无效的链接地址";
                    db.Create<ApiLogEntity>(log);
                    return ResultUtil.AuthFail("无效的链接地址");
                }
            }

            // 验证类型和目标
            if (entity.Type != 1 && entity.Type != 2)
            {
                log.IsOk = false;
                log.Message = "不受支持的消息类型，目前仅支持 1手机号码 | 2微信OpenId";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("不受支持的消息类型，目前仅支持 1手机号码 | 2微信OpenId");
            }

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

            if (_targets.Count > 50)
            {
                log.IsOk = false;
                log.Message = "每次推送必须小于等于50条";
                db.Create<ApiLogEntity>(log);
                return ResultUtil.AuthFail("每次推送必须小于等于50条");
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

            // 流程：写入到OMessage表，发布到cap
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
        /// 消息
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
        
        public void OMessageOperate(OMessageQueueDto dto)
        {

        }
    }
}
