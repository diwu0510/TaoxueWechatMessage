using HZC.Database;
using System;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 微信用户
    /// </summary>
    public class WxUserEntity : BaseEntity
    {
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImage { get; set; }

        /// <summary>
        /// 要忽略的平台,Json
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public string IgnorePlate { get; set; }

        /// <summary>
        /// 是否已关注
        /// </summary>
        public bool IsDescribe { get; set; }
    }
}
