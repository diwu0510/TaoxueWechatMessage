using System.ComponentModel;

namespace Taoxue.Mp.Sms.Services
{
    public enum WxMobileNoticeResultCode
    {
        [Description("待处理")]
        Waiting = 0,
        [Description("处理成功")]
        Success = 200,
        [Description("处理失败")]
        Fail = -1
    }
}
