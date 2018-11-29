using System;
using System.Collections.Generic;
using System.Text;

namespace Taoxue.Mp.Sms.Services
{
    public class OMessageCreateDto
    {
        /// <summary>
        /// 平台保存后的Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 第三方平台传过来的分组标识
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 消息类型，1 Mobile | 2 OpenId
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 手机号码或OpenId
        /// </summary>
        public string[] Target { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string[] Content { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 第三方平台Id
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// 模板Id
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime SendAt { get; set; }
    }

    public class OMessageQueueDto
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string Target { get; set; }

        public string[] Content { get; set; }

        public DateTime SendAt { get; set; }
    }
}
