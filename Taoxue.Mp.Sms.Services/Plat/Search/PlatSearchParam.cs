using HZC.Database;

namespace Taoxue.Mp.Sms.Services
{
    public class PlatSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public bool? Enabled { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().OrderByDesc("UpdateAt");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains("Name", Key.Trim());
            }

            if (Enabled.HasValue)
            {
                util.AndEqual("Enabled", Enabled.Value);
            }

            return util;
        }
    }
}
