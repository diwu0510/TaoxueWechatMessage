using Newtonsoft.Json.Converters;

namespace HZC.Core
{
    /// <summary>
    /// JSON.NET的日期时间格式转换
    /// /// 用法：[JsonConverter(typeof(DateTimeFormatConverter))]
    /// </summary>
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}
