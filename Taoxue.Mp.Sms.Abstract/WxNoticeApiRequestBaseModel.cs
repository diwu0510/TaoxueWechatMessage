using System;

namespace Taoxue.Mp.Sms.Abstract
{
    public class WxNoticeApiRequestBaseModel
    {
        /// <summary>
        /// 第三方平台ID
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// 模板Id
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// 消息类型，取值只能是 1或者2；1为手机号码，2为OpenId
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendAt { get; set; }

        /// <summary>
        /// 消息标签，可用于群发消息的分组筛选
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }
    }
}
