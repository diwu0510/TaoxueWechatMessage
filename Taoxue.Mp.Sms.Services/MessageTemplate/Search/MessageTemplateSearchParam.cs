using HZC.Database;

namespace Taoxue.Mp.Sms.Services
{
    public class MessageTemplateSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public bool? Enabled { get; set; }

        public int? PlatId { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().OrderByDesc("UpdateAt");

            if (Enabled.HasValue)
            {
                util.AndEqual("Enabled", Enabled.Value);
            }

            if (PlatId.HasValue)
            {
                util.AndEqual("PlatId", PlatId.Value);
            }

            return util;
        }
    }
}
