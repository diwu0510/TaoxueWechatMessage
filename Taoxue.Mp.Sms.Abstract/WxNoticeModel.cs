using System;
using System.Collections.Generic;
using System.Text;

namespace Taoxue.Mp.Sms.Abstract
{
    /// <summary>
    /// 微信消息内容模型
    /// </summary>
    public class WxNoticeContentModel
    {
        /// <summary>
        /// 用户标识，手机号码或OpenId
        /// </summary>
        public string UserIdentity { get; set; }

        /// <summary>
        /// 模板消息的内容
        /// </summary>
        public string[] Content { get; set; }

        /// <summary>
        /// 模板消息的连接地址
        /// </summary>
        public string Url { get; set; }
    }
}
