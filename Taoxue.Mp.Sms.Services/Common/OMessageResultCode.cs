using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Taoxue.Mp.Sms.Services
{
    public enum OMessageResultCode
    {
        [Description("等待处理")]
        Waiting = 0,
        [Description("推送完成，已添加到通知队列")]
        Success = 200,
        [Description("未找到绑定的OpenId，仅限类型为Mobile的消息")]
        OpenIdNotFound = 411,
        [Description("未知错误")]
        Exception = 500
    }
}
