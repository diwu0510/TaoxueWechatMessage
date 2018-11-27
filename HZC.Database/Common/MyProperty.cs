namespace HZC.Database
{
    /// <summary>
    /// 实体属性的描述
    /// </summary>
    public class MyProperty
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 对应的数据表列名
        /// </summary>
        public string DataBaseColumn { get; set; }

        /// <summary>
        /// 是不是主键
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// 插入时是否忽略该字段
        /// </summary>
        public bool InsertIgnore { get; set; }

        /// <summary>
        /// 更新时是否忽略该字段
        /// </summary>
        public bool UpdateIgnore { get; set; }

        /// <summary>
        /// 插入和更新时忽略该字段
        /// </summary>
        public bool Ignore { get; set; }
    }
}
