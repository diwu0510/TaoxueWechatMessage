using System;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 微信用户日志
    /// </summary>
    public class WxUserLog
    {
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
