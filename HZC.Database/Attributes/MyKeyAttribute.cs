using System;

namespace HZC.Database
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MyKeyAttribute : Attribute
    {
        /// <summary>
        /// 主键对应的数据表列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 该主键内容是否由手动生成
        /// </summary>
        public bool IsManual { get; set; } = false;

        public MyKeyAttribute(string columnName = "", bool isManual = false)
        {
            ColumnName = columnName;
            IsManual = isManual;
        }
    }
}
