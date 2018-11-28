using HZC.Database;
using System;

namespace Taoxue.Mp.Sms.Services
{
    public class OMessageSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int? Type { get; set; }

        public int? PlatId { get; set; }

        public int? ResultCode { get; set; }

        public DateTime? SendAtStart { get; set; }

        public DateTime? SendAtEnd { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().OrderByDesc("Id");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains("Target", Key);
            }

            if (Type.HasValue)
            {
                util.AndEqual("Type", Type.Value);
            }

            if (PlatId.HasValue)
            {
                util.AndEqual("PlatId", PlatId.Value);
            }

            if (ResultCode.HasValue)
            {
                util.AndEqual("ResultCode", ResultCode.Value);
            }

            if (SendAtStart.HasValue && SendAtStart.Value > DateTime.Parse("2000-01-01"))
            {
                util.AndGreaterThanEqual("SendAt", SendAtStart.Value);
            }

            if (SendAtEnd.HasValue && SendAtEnd.Value > DateTime.Parse("2000-01-01"))
            {
                util.AndLessThanEqual("SendAt", SendAtEnd.Value);
            }

            return util;
        }
    }
}
