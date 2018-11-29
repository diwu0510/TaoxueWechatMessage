namespace Taoxue.Mp.Sms.Abstract
{
    public class WxNoticeApiRequestModel : WxNoticeApiRequestBaseModel
    {
        public WxNoticeApiRequestModel()
        {
            Content = new string[] { };
        }

        /// <summary>
        /// 用户标识，手机号码或OpenId
        /// </summary>
        public string UserIdentity { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string[] Content { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string Url { get; set; }
    }
}
