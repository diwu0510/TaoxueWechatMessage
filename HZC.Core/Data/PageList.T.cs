using System.Collections.Generic;

namespace HZC.Core
{
    public class PageList<T>
    {
        public IEnumerable<T> Body { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int RecordCount { get; set; }
    }
}
