using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace HZC.Database
{
    public class MySearchUtil
    {
        public static MySearchUtil New()
        {
            return new MySearchUtil();
        }

        private int _idx = 0;
        private string _sqlParameterPrefix = "@";
        private string _conditionClauses = string.Empty;
        private List<string> _cols = new List<string>();
        private List<string> _orderby = new List<string>();
        private DynamicParameters _params = new DynamicParameters();

        #region 直接指定字符串
        public MySearchUtil And(string conditionString)
        {
            if (!string.IsNullOrWhiteSpace(conditionString))
            {
                if (!string.IsNullOrWhiteSpace(_conditionClauses))
                {
                    _conditionClauses += " AND ";
                }
                _conditionClauses += conditionString;
            }

            return this;
        }

        public MySearchUtil Or(string conditionString)
        {
            if (!string.IsNullOrWhiteSpace(conditionString))
            {
                if (!string.IsNullOrWhiteSpace(_conditionClauses))
                {
                    _conditionClauses = "(" + _conditionClauses + ") OR ";
                }
                _conditionClauses += conditionString;
            }
            return this;
        }

        public MySearchUtil AndOr(string conditionString)
        {
            if (!string.IsNullOrWhiteSpace(conditionString))
            {
                if (!string.IsNullOrWhiteSpace(_conditionClauses))
                {
                    _conditionClauses += " AND ";
                }
                _conditionClauses += "(" + conditionString + ")";
            }
            return this;
        }
        #endregion

        private string BuildWhereClause(string column, string op, object value)
        {
            var paramName = $"p{_idx++}";
            _params.Add(paramName, value);
            return $"{column}{op}{_sqlParameterPrefix}{paramName}";
        }

        #region And
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="column">列名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public MySearchUtil AndEqual(string column, object value)
        {
            return And(BuildWhereClause(column, "=", value));
        }

        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndNotEqual(string column, object value)
        {
            return And(BuildWhereClause(column, "<>", value));
        }

        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndGreaterThan(string column, object value)
        {
            return And(BuildWhereClause(column, ">", value));
        }

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndGreaterThanEqual(string column, object value)
        {
            return And(BuildWhereClause(column, ">=", value));
        }

        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndLessThan(string column, object value)
        {
            return And(BuildWhereClause(column, "<", value));
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndLessThanEqual(string column, object value)
        {
            return And(BuildWhereClause(column, "<=", value));
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndContains(string column, string value)
        {
            return And(BuildWhereClause(column, " LIKE ", $"%{value}%"));
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndContains(string[] columns, string value)
        {
            var paramName = "p" + _idx++.ToString();
            _params.Add(paramName, "%" + value + "%");

            if (columns.Count() > 0)
            {
                if (!string.IsNullOrWhiteSpace(_conditionClauses))
                {
                    _conditionClauses += " AND ";
                }
                _conditionClauses += "(";
                List<string> claus = new List<string>();
                foreach (var column in columns)
                {
                    claus.Add(column + " LIKE " + _sqlParameterPrefix + paramName);
                }

                _conditionClauses += string.Join(" OR ", claus) + ")";
            }

            return this;
        }

        /// <summary>
        /// 左包含
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndStartWith(string column, string value)
        {
            return And(BuildWhereClause(column, " LIKE ", $"{value}%"));
        }

        /// <summary>
        /// 右包含
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndEndWith(string column, string value)
        {
            return And(BuildWhereClause(column, " LIKE ", $"%{value}"));
        }

        /// <summary>
        /// In子句-数字
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, object[] value)
        {
            if (value.Count() == 0)
            {
                return this;
            }
            return And(BuildWhereClause(column, " IN ", value));
        }

        /// <summary>
        /// 不为空
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndNotNull(string column)
        {
            return And($"{column} IS NOT NULL");
        }

        /// <summary>
        /// 不为空且不为空字符串
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndNotNullOrEmptyString(string column)
        {
            return And($"{column} IS NOT NULL AND {column}<>''");
        }

        /// <summary>
        /// 为空
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndIsNull(string column)
        {
            return And($"{column} IS NULL");
        }

        /// <summary>
        /// 为空或空字符串
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndIsNullOrEmptyString(string column)
        {
            return And($"({column} IS NULL Or {column} = '')");
        }
        #endregion

        #region OR
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="column">列名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public MySearchUtil OrEqual(string column, object value)
        {
            return Or(BuildWhereClause(column, "=", value));
        }

        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrNotEqual(string column, object value)
        {
            return Or(BuildWhereClause(column, "<>", value));
        }

        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrGreaterThan(string column, object value)
        {
            return Or(BuildWhereClause(column, ">", value));
        }

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrGreaterThanEqual(string column, object value)
        {
            return Or(BuildWhereClause(column, ">=", value));
        }

        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrLessThan(string column, object value)
        {
            return Or(BuildWhereClause(column, "<", value));
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrLessThanEqual(string column, object value)
        {
            return Or(BuildWhereClause(column, "<=", value));
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrContains(string column, string value)
        {
            return Or(BuildWhereClause(column, " LIKE ", $"%{value}%"));
        }

        /// <summary>
        /// 左包含
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrStartWith(string column, string value)
        {
            return Or(BuildWhereClause(column, " LIKE ", $"{value}%"));
        }

        /// <summary>
        /// 右包含
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrEndWith(string column, string value)
        {
            return Or(BuildWhereClause(column, " LIKE ", $"%{value}"));
        }

        /// <summary>
        /// In子句-数字
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil OrIn(string column, object[] value)
        {
            if (value.Count() == 0)
            {
                return this;
            }
            return Or(BuildWhereClause(column, " IN ", value));
        }

        /// <summary>
        /// 不为空
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil OrNotNull(string column)
        {
            return Or($"{column} IS NOT NULL");
        }

        /// <summary>
        /// 不为空且不为空字符串
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil OrNotNullOrEmptyString(string column)
        {
            return Or($"{column} IS NOT NULL AND {column}<>''");
        }

        /// <summary>
        /// 为空
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil OrIsNull(string column)
        {
            return Or($"{column} IS NULL");
        }

        /// <summary>
        /// 为空或空字符串
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil OrIsNullOrEmptyString(string column)
        {
            return Or($"({column} IS NULL Or {column} = '')");
        }
        #endregion

        #region 排序
        public MySearchUtil OrderBy(string column)
        {
            _orderby.Add(column);
            return this;
        }

        public MySearchUtil OrderByDesc(string column)
        {
            _orderby.Add(column + " DESC");
            return this;
        }
        #endregion

        public string GetConditionClaus()
        {
            if (string.IsNullOrWhiteSpace(_conditionClauses))
            {
                return "1=1";
            }
            else
            {
                return _conditionClauses;
            }
        }

        public DynamicParameters GetParameters()
        {
            return _params;
        }

        public DynamicParameters GetPageListParameters()
        {
            _params.Add("RecordCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            return _params;
        }

        public string GetOrderByClaus()
        {
            if (_orderby.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(",", _orderby);
            }
        }
    }
}
