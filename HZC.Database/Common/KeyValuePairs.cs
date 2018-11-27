using System.Collections.Generic;

namespace HZC.Database
{
    public class KeyValuePairs : List<KeyValuePair<string, object>>
    {
        public static KeyValuePairs New()
        {
            return new KeyValuePairs();
        }

        public KeyValuePairs Add(string column, object value)
        {
            this.Add(new KeyValuePair<string, object>(column, value));
            return this;
        }
    }
}
