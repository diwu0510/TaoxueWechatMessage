using HZC.Database;
using System;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 用户手机绑定
    /// </summary>
    [MyDataTable("Base_WxUserMobile")]
    public class WxUserMobile
    {
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 绑定时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
