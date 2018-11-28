using HZC.Database;

namespace Taoxue.Mp.Sms.Services
{
    public interface ISearchParam
    {
        MySearchUtil ToSearchUtil();
    }

    public abstract class SearchParam : ISearchParam
    {
        public string OrderBy { get; set; } = "Id";

        public string OrderSort { get; set; } = "DESC";

        public abstract MySearchUtil ToSearchUtil();

        protected string SetOrderBy()
        {
            OrderBy = string.IsNullOrWhiteSpace(OrderBy) ? "Id" : OrderBy;

            if (!string.IsNullOrWhiteSpace(OrderSort) && OrderSort.ToUpper() == "ASC")
            {
                OrderSort = " ASC";
            }
            else
            {
                OrderSort = " DESC";
            }

            return $"{OrderBy}{OrderSort}";
        }
    }
}
