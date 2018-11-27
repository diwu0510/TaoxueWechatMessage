using System.Collections.Generic;

namespace HZC.Database
{
    public class MyEntity
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 对应的数据表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 主键名称
        /// </summary>
        public string EntityKeyName { get; set; }

        /// <summary>
        /// 数据表主键列
        /// </summary>
        public string TableKeyColumn { get; set; }

        /// <summary>
        /// 属性描述列表
        /// </summary>
        public List<MyProperty> Properties { get; set; }

        /// <summary>
        /// 插入的sql语句
        /// </summary>
        public string InsertSqlStatement { get; set; }

        /// <summary>
        /// 修改的sql语句
        /// </summary>
        public string UpdateSqlStatement { get; set; }

        /// <summary>
        /// 是否记录版本
        /// </summary>
        public bool HasVersion { get; set; } = false;
    }
}
