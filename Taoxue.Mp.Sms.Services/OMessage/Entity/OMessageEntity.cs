using HZC.Database;
using System;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 原始消息（待发送消息）
    /// </summary>
    [MyDataTable("Message_OMessage")]
    public class OMessageEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// 目标类型，1 Mobile，2 OpenId
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 消息推送目标，手机号码或者OpenId
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 第三方平台ID
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// 消息内容，Json字符串
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息链接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 第三方平台上的消息分组标识
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendAt { get; set; }

        /// <summary>
        /// 处理结果编码
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

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
