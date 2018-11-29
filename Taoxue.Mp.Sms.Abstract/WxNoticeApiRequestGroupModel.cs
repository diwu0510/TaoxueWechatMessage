using System.Collections.Generic;

namespace Taoxue.Mp.Sms.Abstract
{
    public class WxNoticeApiRequestGroupModel : WxNoticeApiRequestBaseModel
    {
        public WxNoticeApiRequestGroupModel()
        {
            Contents = new List<WxNoticeContentModel>();
        }

        /// <summary>
        /// 消息内容主体
        /// </summary>
        public List<WxNoticeContentModel> Contents { get; set; }
    }
}
