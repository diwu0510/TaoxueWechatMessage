using Dapper;
using HZC.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HZC.Database
{
    public partial class MyDbUtil
    {
        private static IConfiguration _configuration;
        private string _connectionString;
        private string _paramPrefix = "@";

        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region 构造函数
        public MyDbUtil(string sectionName = "")
        {
            if (_configuration == null)
            {
                throw new Exception("数据库工具未初始化，要在Startup.cs中调用MyDbUtil.Init(Configuration);进行注册");
            }
            sectionName = string.IsNullOrWhiteSpace(sectionName) ? "DefaultConnectionString" : sectionName.Trim();
            _connectionString = _configuration.GetConnectionString(sectionName);
        }
        #endregion

        #region 获取数据库连接
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
        #endregion

        #region 获取查询字符串
        public string GetQuerySql(string cols, string table, string where, string orderby, int? top = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            if (top.HasValue)
            {
                sb.Append("TOP ")
                    .Append(top.Value)
                    .Append(" ");
            }
            sb.Append(cols);
            sb.Append(" FROM ");
            sb.Append(table);
            sb.Append(" WHERE ");
            sb.Append(where);
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                sb.Append(" ORDER BY ");
                sb.Append(orderby);
            }
            return sb.ToString();
        }

        public string GetPagingQuerySql(string cols, string tables, string condition, string orderby, int index, int size)
        {
            if (index == 1)
            {
                string sql = $"SELECT TOP {size} {cols} FROM {tables} WHERE {condition} ORDER BY {orderby};SELECT {_paramPrefix}RecordCount=COUNT(0) FROM {tables} WHERE {condition}";

                return sql;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("FROM ").Append(tables);

                if (!string.IsNullOrWhiteSpace(condition))
                {
                    sb.Append(" WHERE ").Append(condition);
                }

                if (string.IsNullOrWhiteSpace(orderby))
                {
                    throw new Exception("分页查询必须提供OrderBy字段");
                }

                string sql = string.Format(
                    @"  WITH PAGEDDATA AS
					    (
						    SELECT TOP 100 PERCENT {0}, ROW_NUMBER() OVER (ORDER BY {1}) AS FLUENTDATA_ROWNUMBER
						    {2}
					    )
					    SELECT {0}
					    FROM PAGEDDATA
					    WHERE FLUENTDATA_ROWNUMBER BETWEEN {3} AND {4};
                        SELECT {7}RecordCount=COUNT(0) FROM {5} WHERE {6}",
                    cols,
                    orderby,
                    sb,
                    (index - 1) * size + 1,
                    index * size,
                    tables,
                    condition,
                    _paramPrefix
                );
                return sql;
            }
        }
        #endregion

        #region 获取所有数据
        /// <summary>
        /// 通过SQL获取所有数据
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">查询参数</param>
        /// <returns></returns>
        public IEnumerable<T> FetchBySql<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param);
            }
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="util">查询参数</param>
        /// <param name="table">查询的数据表</param>
        /// <param name="cols">要查询的列</param>
        /// <param name="top">要获取的数量</param>
        /// <returns></returns>
        public IEnumerable<T> Fetch<T>(MySearchUtil util, string table = "", string cols = "*", int? top = null)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                table = GetTableName(typeof(T));
            }

            string where = util.GetConditionClaus();
            string orderby = util.GetOrderByClaus();
            DynamicParameters param = util.GetParameters();

            string sql = GetQuerySql(cols, table, where, orderby, top);
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param);
            }
        }

        /// <summary>
        /// 通过sql语句获取dynamic类型数据列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> FetchBySql(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, param);
            }
        }

        /// <summary>
        /// 获取指定表的所有数据，返回dynamic类型的数据列表
        /// </summary>
        /// <param name="util">查询参数</param>
        /// <param name="table">要查询的表</param>
        /// <param name="cols">要查询的列</param>
        /// <param name="top">指定数量</param>
        /// <returns></returns>
        public IEnumerable<dynamic> Fetch(MySearchUtil util, string table, string cols = "*", int? top = null)
        {
            string where = util.GetConditionClaus();
            string orderby = util.GetOrderByClaus();
            DynamicParameters param = util.GetParameters();

            string sql = GetQuerySql(cols, table, where, orderby);
            using (var conn = GetConnection())
            {
                return conn.Query(sql, param);
            }
        }
        #endregion

        #region 分页查询
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="util">查询参数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="table">要查询的表</param>
        /// <param name="cols">要查询的列</param>
        /// <returns></returns>
        public PageList<T> Query<T>(MySearchUtil util, int pageIndex, int pageSize, string table = "", string cols = "*")
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                table = GetTableName(typeof(T));
            }

            string where = util.GetConditionClaus();
            string orderby = util.GetOrderByClaus();
            DynamicParameters param = util.GetPageListParameters();

            string sql = GetPagingQuerySql(cols, table, where, orderby, pageIndex, pageSize);
            using (var conn = GetConnection())
            {
                var list = conn.Query<T>(sql, param);
                var total = param.Get<int>("RecordCount");
                return new PageList<T>
                {
                    Body = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    RecordCount = total
                };
            }
        }

        /// <summary>
        /// 获取分页动态数据列表
        /// </summary>
        /// <param name="util">查询参数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="table">要查询的表</param>
        /// <param name="cols">要查询的列</param>
        /// <returns></returns>
        public PageList<dynamic> Query(MySearchUtil util, int pageIndex, int pageSize, string table, string cols = "*")
        {
            string where = util.GetConditionClaus();
            string orderby = util.GetOrderByClaus();
            DynamicParameters param = util.GetPageListParameters();

            string sql = GetPagingQuerySql(cols, table, where, orderby, pageIndex, pageSize);
            using (var conn = GetConnection())
            {
                var list = conn.Query(sql, param);
                var total = param.Get<int>("RecordCount");
                return new PageList<dynamic>
                {
                    Body = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    RecordCount = total
                };
            }
        }
        #endregion

        #region 加载实体
        /// <summary>
        /// 加载实体
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public dynamic LoadBySql(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query(sql, param).FirstOrDefault();
            }
        }

        /// <summary>
        /// 加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadBySql<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param).FirstOrDefault();
            }
        }

        /// <summary>
        /// 加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public T Load<T>(int id, string cols = "*")
        {
            using (var conn = GetConnection())
            {
                string sql = "SELECT " + cols + " FROM [" + GetTableName(typeof(T)) + "] WHERE Id=" + _paramPrefix + "id";
                return conn.Query<T>(sql, new { id = id }).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="util"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public T Load<T>(MySearchUtil util, string cols = "*")
        {
            using (var conn = GetConnection())
            {
                string sql = "SELECT TOP 1 " + cols + " FROM [" + GetTableName(typeof(T)) + "] WHERE " +
                    util.GetConditionClaus() + (string.IsNullOrWhiteSpace(util.GetOrderByClaus()) ? "" : " ORDER BY " + util.GetOrderByClaus());
                return conn.Query<T>(sql, util.GetParameters()).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体，包含指定导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, P1>(string sql, Func<T, P1, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T, P1, T>(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体，包含指定导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, P1, P2>(string sql, Func<T, P1, P2, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T, P1, P2, T>(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体，包含指定导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <typeparam name="P3"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, P1, P2, P3>(string sql, Func<T, P1, P2, P3, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T, P1, P2, P3, T>(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体，包含指定导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <typeparam name="P3"></typeparam>
        /// <typeparam name="P4"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, P1, P2, P3, P4>(string sql, Func<T, P1, P2, P3, P4, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T, P1, P2, P3, P4, T>(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体，包含指定导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <typeparam name="P3"></typeparam>
        /// <typeparam name="P4"></typeparam>
        /// <typeparam name="P5"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadJoin<T, P1, P2, P3, P4, P5>(string sql, Func<T, P1, P2, P3, P4, P5, T> func, string splitOn, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T, P1, P2, P3, P4, P5, T>(sql, func, param, splitOn: splitOn).SingleOrDefault();
            }
        }

        /// <summary>
        /// 加载实体，包含指定导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T LoadWith<T, T1>(string sql, Func<T, List<T1>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1s = multiReader.Read<T1>().ToList();
                    func(entity, sub1s);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2>(string sql, Func<T, List<T1>, List<T2>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1s = multiReader.Read<T1>().ToList();
                    var sub2s = multiReader.Read<T2>().ToList();
                    func(entity, sub1s, sub2s);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2, T3>(string sql, Func<T, List<T1>, List<T2>, List<T3>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1s = multiReader.Read<T1>().ToList();
                    var sub2s = multiReader.Read<T2>().ToList();
                    var sub3s = multiReader.Read<T3>().ToList();
                    func(entity, sub1s, sub2s, sub3s);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2, T3, T4>(string sql, Func<T, List<T1>, List<T2>, List<T3>, List<T4>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1s = multiReader.Read<T1>().ToList();
                    var sub2s = multiReader.Read<T2>().ToList();
                    var sub3s = multiReader.Read<T3>().ToList();
                    var sub4s = multiReader.Read<T4>().ToList();
                    func(entity, sub1s, sub2s, sub3s, sub4s);
                    return entity;
                }
            }
        }

        public T LoadWith<T, T1, T2, T3, T4, T5>(string sql, Func<T, List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, T> func, Object param = null)
        {
            using (var conn = GetConnection())
            {
                using (var multiReader = conn.QueryMultiple(sql, param))
                {
                    var entity = multiReader.Read<T>().SingleOrDefault();
                    var sub1s = multiReader.Read<T1>().ToList();
                    var sub2s = multiReader.Read<T2>().ToList();
                    var sub3s = multiReader.Read<T3>().ToList();
                    var sub4s = multiReader.Read<T4>().ToList();
                    var sub5s = multiReader.Read<T5>().ToList();
                    func(entity, sub1s, sub2s, sub3s, sub4s, sub5s);
                    return entity;
                }
            }
        }
        #endregion

        #region 获取数量
        public int GetCount<T>(MySearchUtil util = null)
        {
            string condition = "";
            DynamicParameters param = null;

            if (util != null)
            {
                condition = util.GetConditionClaus();
                param = util.GetParameters();
            }
            else
            {
                condition = "1=1";
            }

            var tableName = GetTableName(typeof(T));

            using (var conn = GetConnection())
            {
                string sql = "SELECT COUNT(0) FROM [" + tableName + "] WHERE " + condition;
                return conn.ExecuteScalar<int>(sql, param);
            }
        }

        public int GetCount(string tableName, MySearchUtil util = null)
        {
            string condition = "";
            DynamicParameters param = null;

            if (util != null)
            {
                condition = util.GetConditionClaus();
                param = util.GetParameters();
            }
            else
            {
                condition = "1=1";
            }

            using (var conn = GetConnection())
            {
                string sql = "SELECT COUNT(0) FROM [" + tableName + "] WHERE " + condition;
                return conn.ExecuteScalar<int>(sql, param);
            }
        }
        #endregion

        #region 创建
        public int Create<T>(T t)
        {
            using (var conn = GetConnection())
            {
                string sql = MyContainer.Get(typeof(T)).InsertSqlStatement;
                return conn.ExecuteScalar<int>(sql, t);
            }
        }

        public int Create<T>(List<T> ts)
        {
            if (ts.Count == 0) return 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = MyContainer.Get(typeof(T)).InsertSqlStatement;
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var rows = conn.Execute(sql, ts, trans, 30, CommandType.Text);
                        trans.Commit();
                        return rows;
                    }
                    catch (DataException ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region 更新
        public int Update<T>(T t)
        {
            using (var conn = GetConnection())
            {
                string sql = MyContainer.Get(typeof(T)).UpdateSqlStatement;
                return conn.Execute(sql, t);
            }
        }

        public int Update<T>(List<T> ts)
        {
            if (ts.Count == 0) return 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = MyContainer.Get(typeof(T)).UpdateSqlStatement;
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var rows = conn.Execute(sql, ts.ToArray(), trans, 30, CommandType.Text);
                        trans.Commit();
                        return rows;
                    }
                    catch (DataException ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public int Update<T>(T entity, string[] columns)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = $"UPDATE {GetTableName(typeof(T))} SET ";
                foreach (var col in columns)
                {
                    sql += $"{col}=@{col},";
                }
                sql.TrimEnd(',');
                sql += " WHERE Id=@Id";

                return Execute(sql, entity);
            }
        }
        #endregion

        #region 更新指定字段
        public int Update<T>(object obj, MySearchUtil util)
        {
            using (var conn = GetConnection())
            {
                string where = util.GetConditionClaus();
                var param = util.GetParameters();

                var props = obj.GetType().GetProperties();
                var entity = MyContainer.Get(typeof(T));

                StringBuilder sb = new StringBuilder();
                List<string> _cols = new List<string>();

                List<string> propNames = new List<string>();
                foreach (var p in props)
                {
                    propNames.Add(p.Name);
                    param.Add(p.Name, p.GetValue(obj));
                    _cols.Add($"{p.Name}=@{p.Name}");
                }

                sb.Append($"UPDATE [{entity.TableName}] SET ");
                sb.Append(string.Join(',', _cols));
                sb.Append($" WHERE {where}");

                return conn.Execute(sb.ToString(), obj);
            }
        }

        public int Update<T>(KeyValuePairs cols, MySearchUtil util)
        {
            using (var conn = GetConnection())
            {
                var _params = util.GetParameters();
                List<string> _update_cols = new List<string>();
                int idx = 0;
                string _paramName = "";
                foreach (var kv in cols)
                {
                    _paramName = "@c" + idx.ToString();
                    _update_cols.Add(kv.Key + "=" + _paramName);
                    _params.Add(_paramName, kv.Value);
                    idx++;
                }
                string table = GetTableName(typeof(T));
                string sql = "UPDATE [" + table + "] SET " + string.Join(",", _update_cols) + " WHERE " + util.GetConditionClaus();
                return conn.Execute(sql, _params);
            }
        }

        public int Update(KeyValuePairs cols, string table, MySearchUtil util)
        {
            using (var conn = GetConnection())
            {
                var _params = util.GetParameters();
                List<string> _update_cols = new List<string>();
                int idx = 0;
                string _paramName = "";
                foreach (var kv in cols)
                {
                    _paramName = _paramPrefix + "c" + idx.ToString();
                    _update_cols.Add(kv.Key + "=" + _paramName);
                    _params.Add(_paramName, kv.Value);
                    idx++;
                }
                string sql = "UPDATE [" + table + "] SET " + string.Join(",", _update_cols) + " WHERE " + util.GetConditionClaus();
                return conn.Execute(sql, _params);
            }
        }
        #endregion

        #region 删除
        public int Delete<T>(int id, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }
                return conn.Execute($"DELETE [{tableName}] WHERE Id={_paramPrefix}id", new { Id = id });
            }
        }

        public int Delete<T>(int[] ids, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }

                return conn.Execute($"DELETE [{tableName}] WHERE Id in @Ids", new { Ids = ids });
            }
        }

        public int Delete<T>(MySearchUtil util, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (util == null)
                {
                    util = new MySearchUtil();
                }

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }

                return conn.Execute($"DELETE [{tableName}] WHERE {util.GetConditionClaus()}", util.GetParameters());
            }
        }

        public int Remove<T>(int id, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }
                return conn.Execute(string.Format(@"UPDATE [{0}] SET IsDel=1 WHERE Id=" + _paramPrefix + "id", tableName), new { id = id });
            }
        }

        public int Remove<T>(int[] ids, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }
                return conn.Execute(string.Format(@"UPDATE [{0}] SET IsDel=1 WHERE Id in @ids", tableName), new { ids = ids });
            }
        }

        public int Remove<T>(MySearchUtil mcu, string tableName = "")
        {
            using (var conn = GetConnection())
            {
                if (mcu == null)
                {
                    mcu = new MySearchUtil();
                }

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    tableName = GetTableName(typeof(T));
                }

                return conn.Execute(string.Format(@"UPDATE [{0}] SET IsDel=1 WHERE ", tableName, mcu.GetConditionClaus()), mcu.GetParameters());
            }
        }
        #endregion

        #region 执行SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string sql, Object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Execute(sql, param);
            }
        }

        /// <summary>
        /// 执行并获取第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<T>(sql, param);
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteProc(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Execute(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行存储过程并获取第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteProc<T>(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<T>(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTran(string[] sqls)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var s in sqls)
                        {
                            conn.Execute(s, null, tran);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTran(KeyValuePairs sqls)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var s in sqls)
                        {
                            conn.Execute(s.Key, s.Value, tran);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> MultiQuery<T1, T2>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(list1, list2);
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> MultiQuery<T1, T2, T3>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(list1, list2, list3);
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> MultiQuery<T1, T2, T3, T4>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();
                    var list4 = multi.Read<T4>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(list1, list2, list3, list4);
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> MultiQuery<T1, T2, T3, T4, T5>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();
                    var list4 = multi.Read<T4>();
                    var list5 = multi.Read<T5>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(list1, list2, list3, list4, list5);
                }
            }
        }
        #endregion

        #region 获取通用增改字符串
        public string GetCommonInsertSqlStatement<T>()
        {
            try
            {
                return MyContainer.Get(typeof(T)).InsertSqlStatement;
            }
            catch
            {
                throw new Exception("指定类型未在MyContainer中注册");
            }
        }

        public string GetCommonUpdateSqlStatement<T>()
        {
            try
            {
                return MyContainer.Get(typeof(T)).UpdateSqlStatement;
            }
            catch
            {
                throw new Exception("指定类型未在MyContainer中注册");
            }
        }

        public string GetInsertSqlStatement<T>(object obj)
        {
            try
            {
                var entity = MyContainer.Get(typeof(T));
                var sql = MyEntityUtil.BuildInsertSqlByAnonymous(entity, obj);
                return sql;
            }
            catch
            {
                throw new Exception("指定类型未在MyContainer中注册");
            }
        }

        public string GetInsertSqlStatement<T>(string[] columns, bool isExclude = false)
        {
            try
            {
                var entity = MyContainer.Get(typeof(T));
                var sql = MyEntityUtil.BuildInsertSqlStatement(entity, columns, isExclude);
                return sql;
            }
            catch
            {
                throw new Exception("指定类型未在MyContainer中注册");
            }
        }

        public string GetUpdateSqlStatement<T>(string[] columns, bool isExclude = false, MySearchUtil util = null)
        {
            try
            {
                var entity = MyContainer.Get(typeof(T));
                var sql = MyEntityUtil.BuildUpdateSqlStatement(entity, columns, isExclude, util == null ? "" : util.GetConditionClaus());
                return sql;
            }
            catch
            {
                throw new Exception("指定类型未在MyContainer中注册");
            }
        }

        public string GetUpdateSqlStatement<T>(object obj, MySearchUtil util = null)
        {
            try
            {
                var entity = MyContainer.Get(typeof(T));
                var sql = MyEntityUtil.BuildUpdateSqlByAnonymous(entity, obj, util == null ? "" : util.GetConditionClaus());
                return sql;
            }
            catch
            {
                throw new Exception("指定类型未在MyContainer中注册");
            }
        }
        #endregion

        #region 获取表名称
        public string GetTableName(Type type)
        {
            var entityInfo = MyContainer.Get(type);
            return entityInfo.TableName;
        }
        #endregion
    }
}
