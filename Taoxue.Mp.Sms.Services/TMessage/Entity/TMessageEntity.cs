using HZC.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 待发送的模板消息
    /// </summary>
    [MyDataTable("Message_TMessage")]
    public class TMessageEntity : BaseEntity
    {
        /// <summary>
        /// 原始消息ID
        /// </summary>
        public int OMessageId { get; set; }

        /// <summary>
        /// 推送目标的OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 推送内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 队列处理结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 队列处理完成时间
        /// </summary>
        public DateTime FinishAt { get; set; }

        /// <summary>
        /// 微信返回的推送结果
        /// </summary>
        public string ResponseResult { get; set; }

        /// <summary>
        /// 微信返回的消息ID
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 接收到微信返回的时间
        /// </summary>
        public DateTime ResponseAt { get; set; }
    }
}
