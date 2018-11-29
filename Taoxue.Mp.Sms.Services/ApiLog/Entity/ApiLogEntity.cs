using System;

namespace Taoxue.Mp.Sms.Services
{
    public class ApiLogEntity
    {
        /// <summary>
        /// LogId
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 请求的接口
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// 请求的IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 消息分组标识
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 平台ID
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// 请求方式，GET|POST
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求参数，Json
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否请求成功
        /// </summary>
        public bool IsOk { get; set; }

        /// <summary>
        /// 若请求失败，记录失败原因
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
