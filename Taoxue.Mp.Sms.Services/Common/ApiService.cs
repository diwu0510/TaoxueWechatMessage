using DotNetCore.CAP;
using DotNetCore.CAP.Infrastructure;
using HZC.Core;
using HZC.Database;
using HZC.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Taoxue.Mp.Sms.Abstract;

namespace Taoxue.Mp.Sms.Services
{
    public class ApiService
    {

        private readonly MyDbUtil db;
        private readonly ICapPublisher _publisher;

        public ApiService(ICapPublisher publisher, string sectionName = "")
        {
            db = new MyDbUtil(sectionName);
            _publisher = publisher;
        }

        public Result ApiRequestHandler(WxNoticeApiRequestModel model)
        {
            // 验证参数有效性
            if (model.PlatId <= 0 ||
                model.TemplateId <= 0 ||
                model.SendAt == null || model.SendAt < DateTime.Today ||
                (model.Type != 1 && model.Type != 2))
            {
                return ResultUtil.AuthFail("请求参数无效，可能原因为：1、PlatId|TemplateId小于等于0；2、发送日期不合法或小于当天日期；3、消息类型不等于1或2（当前仅支持1|2两种取值）");
            }

            // 验证平台有效性
            var plat = PlatUtil.Get(model.PlatId);
            if (plat == null || !plat.Enabled)
            {
                return ResultUtil.AuthFail("接入平台不存在或该平台已被禁用");
            }

            // 验证模板有效性
            var temp = MessageTemplateUtil.Get(model.TemplateId);
            if (temp == null || !temp.Enabled)
            {
                return ResultUtil.AuthFail("模板不存在或已被禁用");
            }

            // 验证签名
            var sign = $"{model.SendAt.ToString("yyyyMMddHHmmss")}-{plat.SecretKey}";
            sign = MD5EncryptUtil.ConvertMD5(sign);

            if (sign != model.Sign)
            {
                return ResultUtil.AuthFail("签名验证失败");
            }

            if (model.Type == 1)
            {
                // 手机消息处理
                var entity = WxNoticeRequest2MobileNotice(model, plat.Name, temp.TemplateId);

                using (var conn = db.GetConnection())
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var id = db.Create<WxMobileNoticeEntity>(entity);

                            _publisher.Publish("Taoxue.Sms.MobileNotice.Create", WxMobileNotice2QueueDto(id, temp.TemplateId, model));

                            trans.Commit();
                            return ResultUtil.Success();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            conn.Close();
                            return ResultUtil.Exception(ex);
                            throw ex;
                        }
                    }
                }
            }
            else if (model.Type == 2)
            {
                // OpenId消息处理
                var entity = WxNoticeRequest2Notice(model, plat.Name, temp.TemplateId);

                using (var conn = db.GetConnection())
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var id = db.Create<WxNoticeEntity>(entity);

                            _publisher.Publish("Taoxue.Sms.Notice.Create", WxNotice2QueueDto(id, temp.TemplateId, model));

                            trans.Commit();
                            return ResultUtil.Success();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            conn.Close();
                            return ResultUtil.Exception(ex);
                            throw ex;
                        }
                    }
                }
            }
            else
            {
                return ResultUtil.Fail("未知消息类型");
            }
        }

        #region 私有方法-转换各种Dto
        private WxMobileNoticeEntity WxNoticeRequest2MobileNotice(WxNoticeApiRequestModel model, string platName, string templateId)
        {
            var entity = new WxMobileNoticeEntity
            {
                Content = JsonConvert.SerializeObject(model.Content),
                PlatId = model.PlatId,
                PlatName = platName,
                TemplateId = templateId,
                Url = model.Url,
                Tag = model.Tag,
                Remark = "",
                CreateAt = DateTime.Now,
                ResultCode = (int)WxMobileNoticeResultCode.Waiting,
                ResultMessage = WxMobileNoticeResultCode.Waiting.GetDescription(),
                Mobile = model.UserIdentity
            };
            return entity;
        }

        private WxNoticeEntity WxNoticeRequest2Notice(WxNoticeApiRequestModel model, string platName, string templateId)
        {
            var entity = new WxNoticeEntity
            {
                Content = JsonConvert.SerializeObject(model.Content),
                PlatId = model.PlatId,
                PlatName = platName,
                TemplateId = templateId,
                Url = model.Url,
                Tag = model.Tag,
                Remark = "",
                CreateAt = DateTime.Now,
                ResultCode = (int)WxMobileNoticeResultCode.Waiting,
                ResultMessage = WxMobileNoticeResultCode.Waiting.GetDescription(),
                OpenId = model.UserIdentity
            };
            return entity;
        }

        private WxMobileNoticeQueueDto WxMobileNotice2QueueDto(int id, string templateId, WxNoticeApiRequestModel model)
        {
            return new WxMobileNoticeQueueDto
            {
                Id = id,
                Mobile = model.UserIdentity,
                Content = model.Content,
                PlatId = model.PlatId,
                Tag = model.Tag,
                TemplateId = templateId,
                Url = model.Url
            };
        }

        private WxNoticeQueueDto WxNotice2QueueDto(int id, string templateId, WxNoticeApiRequestModel model)
        {
            return new WxNoticeQueueDto
            {
                Id = id,
                OpenId = model.UserIdentity,
                Content = model.Content,
                PlatId = model.PlatId,
                Tag = model.Tag,
                TemplateId = templateId,
                Url = model.Url
            };
        } 
        #endregion
    }
    
    public class WxNoticeBaseEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 内容，序列化的文本数组
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 平台ID
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatName { get; set; }

        /// <summary>
        /// 微信模板ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 消息组的标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// 处理结果说明
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// 处理完成时间
        /// </summary>
        public DateTime? FinishAt { get; set; }
    }

    public class WxMobileNoticeEntity : WxNoticeBaseEntity
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 对应的OpenId数量
        /// </summary>
        public string Users { get; set; }
    }

    public class WxNoticeEntity : WxNoticeBaseEntity
    {
        /// <summary>
        /// 外链，手机消息Id
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信返回的MessageId
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 微信反馈时间
        /// </summary>
        public DateTime? WxResponseAt { get; set; }
    }

    public class WxNoticeQueueBaseDto
    {
        public int Id { get; set; }

        public string[] Content { get; set; }

        public string Tag { get; set; }

        public int PlatId { get; set; }

        public string TemplateId { get; set; }

        public string Url { get; set; }
    }


    public class WxMobileNoticeQueueDto: WxNoticeQueueBaseDto
    {
        public string Mobile { get; set; }
    }

    public class WxNoticeQueueDto :WxNoticeQueueBaseDto
    {
        public string OpenId { get; set; }
    }
}
