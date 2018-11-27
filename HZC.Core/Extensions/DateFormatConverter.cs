using Newtonsoft.Json.Converters;

namespace HZC.Core
{
    /// <summary>
    /// JSON.NET的日期格式转换
    /// 用法：[JsonConverter(typeof(DateFormatConverter))]
    /// </summary>
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
