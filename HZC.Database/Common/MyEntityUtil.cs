using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HZC.Database
{
    public static class MyEntityUtil
    {
        #region 构造INSERT和UPDATE的SQL语句
        /// <summary>
        /// 构造插入语句
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string BuildCommonInsertSqlStatement(MyEntity entity)
        {
            StringBuilder sb = new StringBuilder();
            List<string> _cols = new List<string>();
            List<string> _props = new List<string>();

            foreach (var p in entity.Properties)
            {
                if (!p.Ignore && !p.InsertIgnore && !p.IsPrimary)
                {
                    _cols.Add(p.DataBaseColumn);
                    _props.Add($"@{p.PropertyName}");
                }

            }

            sb.Append($"INSERT INTO [{entity.TableName}] (");
            sb.Append(string.Join(',', _cols));
            sb.Append(") VALUES (");
            sb.Append(string.Join(',', _props));
            sb.Append(");SELECT @@IDENTITY;");

            return sb.ToString();
        }

        /// <summary>
        /// 构造更新语句
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string BuildCommonUpdateSqlStatement(MyEntity entity)
        {
            StringBuilder sb = new StringBuilder();
            List<string> _cols = new List<string>();

            foreach (var p in entity.Properties)
            {
                if (!p.Ignore && !p.UpdateIgnore && !p.IsPrimary)
                {
                    _cols.Add($"{p.DataBaseColumn}=@{p.PropertyName}");
                }
            }

            sb.Append($"UPDATE [{entity.TableName}] SET ");
            sb.Append(string.Join(',', _cols));
            sb.Append($" WHERE {entity.TableKeyColumn}=@{entity.EntityKeyName}");

            if (entity.HasVersion)
            {
                sb.Append(" AND Version=@Version");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 构造插入语句
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="columns">要包含（或忽略）的列</param>
        /// <param name="isExclude">指定列的包含方式。默认为false，既只插入columns指定的列。若设置为true，则插入除columns指定列外的其他列</param>
        /// <returns></returns>
        public static string BuildInsertSqlStatement(MyEntity entity, string[] columns, bool isExclude = false)
        {
            IEnumerable<MyProperty> includeColumns;
            if (isExclude)
            {
                includeColumns = entity.Properties.Where(p => !columns.Contains(p.PropertyName) && !p.Ignore && !p.InsertIgnore);
            }
            else
            {
                includeColumns = entity.Properties.Where(p => columns.Contains(p.PropertyName) && !p.Ignore && !p.InsertIgnore);
            }

            if (includeColumns.Count() == 0)
            {
                throw new ArgumentNullException("columns", "指定字段中未包含有效列");
            }

            StringBuilder sb = new StringBuilder();
            List<string> _cols = new List<string>();
            List<string> _props = new List<string>();

            foreach (var p in includeColumns)
            {
                _cols.Add(p.DataBaseColumn);
                _props.Add($"@{p.PropertyName}");
            }

            sb.Append($"INSERT INTO [{entity.TableName}] (");
            sb.Append(string.Join(',', _cols));
            sb.Append(") VALUES (");
            sb.Append(string.Join(',', _props));
            sb.Append(");SELECT @@IDENTITY;");

            return sb.ToString();
        }

        /// <summary>
        /// 构造查询语句
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="columns">要包含（或忽略的列）</param>
        /// <param name="isExclude">指定列的包含方式。默认为false，既只更新columns指定的列。若设置为true，则更新除columns指定列外的其他列</param>
        /// <returns></returns>
        public static string BuildUpdateSqlStatement(MyEntity entity, string[] columns, bool isExclude = false, string where = "")
        {
            IEnumerable<MyProperty> includeColumns;
            if (isExclude)
            {
                includeColumns = entity.Properties.Where(p => !columns.Contains(p.PropertyName) && !p.Ignore && !p.UpdateIgnore);
            }
            else
            {
                includeColumns = entity.Properties.Where(p => columns.Contains(p.PropertyName) && !p.Ignore && !p.UpdateIgnore);
            }

            if (includeColumns.Count() == 0)
            {
                throw new ArgumentNullException("columns", "指定字段中未包含有效列");
            }

            where = string.IsNullOrWhiteSpace(where) ? "1=1" : where;

            if (includeColumns.Any(c => c.DataBaseColumn == entity.TableKeyColumn))
            {
                where += $"{entity.TableKeyColumn}=@{entity.EntityKeyName}";
            }

            StringBuilder sb = new StringBuilder();
            List<string> _cols = new List<string>();

            foreach (var p in includeColumns)
            {
                _cols.Add($" {p.DataBaseColumn}=@{p.PropertyName}");
            }

            sb.Append($"UPDATE [{entity.TableName}] SET ");
            sb.Append(string.Join(',', _cols));
            sb.Append($" WHERE {where}");

            return sb.ToString();
        }

        /// <summary>
        /// 从匿名类型中获取插入的sql语句
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="obj">匿名对象</param>
        /// <returns></returns>
        public static string BuildInsertSqlByAnonymous(MyEntity entity, object obj)
        {
            var props = obj.GetType().GetProperties().Select(p => p.Name);
            IEnumerable<MyProperty> includeColumns = entity.Properties.Where(p => props.Contains(p.PropertyName) && !p.InsertIgnore && !p.Ignore);

            if (includeColumns.Count() == 0)
            {
                throw new ArgumentNullException("columns", "指定字段中未包含有效列");
            }

            StringBuilder sb = new StringBuilder();
            List<string> _cols = new List<string>();
            List<string> _props = new List<string>();

            foreach (var p in includeColumns)
            {
                _cols.Add(p.DataBaseColumn);
                _props.Add($"@{p.PropertyName}");
            }

            sb.Append($"INSERT INTO [{entity.TableName}] (");
            sb.Append(string.Join(',', _cols));
            sb.Append(") VALUES (");
            sb.Append(string.Join(',', _props));
            sb.Append(");SELECT @@IDENTITY;");

            return sb.ToString();
        }

        /// <summary>
        /// 从匿名类型中获取更新的Sql语句
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="obj">匿名对象</param>
        /// <returns></returns>
        public static string BuildUpdateSqlByAnonymous(MyEntity entity, object obj, string where = "")
        {
            var props = obj.GetType().GetProperties().Select(p => p.Name);
            IEnumerable<MyProperty> includeColumns = entity.Properties.Where(p => props.Contains(p.PropertyName));

            if (includeColumns.Count() == 0)
            {
                throw new ArgumentNullException("columns", "指定字段中未包含有效列");
            }

            where = string.IsNullOrWhiteSpace(where) ? "1=1" : where;

            if (includeColumns.Any(c => c.DataBaseColumn == entity.TableKeyColumn))
            {
                where += $" {entity.TableKeyColumn}=@{entity.EntityKeyName}";
            }

            StringBuilder sb = new StringBuilder();
            List<string> _cols = new List<string>();

            foreach (var p in includeColumns)
            {
                _cols.Add($"{p.DataBaseColumn}=@{p.PropertyName}");
            }

            sb.Append($"UPDATE [{entity.TableName}] SET ");
            sb.Append(string.Join(',', _cols));
            sb.Append($" WHERE {where}");

            return sb.ToString();
        }
        #endregion

        #region PropertyInfo转换为MyPropertyAttribute
        /// <summary>
        /// 将PropertyInfo转换为MyPropertyAttribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static MyEntity ConvertToMyEntity(Type type)
        {
            MyEntity entity = new MyEntity();
            entity.EntityName = type.Name;

            var entityAttribute = type.GetCustomAttribute<MyDataTableAttribute>(false);
            if (entityAttribute != null)
            {
                entity.TableName = entityAttribute.Name;
            }
            else
            {
                entity.TableName = type.Name.Replace("Entity", "");
            }

            entity.Properties = new List<MyProperty>();

            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                if (!CheckPropertyType(p))
                {
                    continue;
                }

                var keyAttr = p.GetCustomAttribute<MyKeyAttribute>(false);
                if (keyAttr != null)
                {
                    var column = string.IsNullOrWhiteSpace(keyAttr.ColumnName) ? p.Name : keyAttr.ColumnName;
                    entity.Properties.Add(new MyProperty
                    {
                        DataBaseColumn = column,
                        IsPrimary = true,
                        InsertIgnore = !keyAttr.IsManual,
                        UpdateIgnore = !keyAttr.IsManual
                    });

                    entity.EntityKeyName = p.Name;
                    entity.TableKeyColumn = column;
                    continue;
                }

                if (p.Name == "Version")
                {
                    entity.Properties.Add(new MyProperty
                    {
                        DataBaseColumn = "Version",
                        InsertIgnore = true,
                        UpdateIgnore = true
                    });
                    entity.HasVersion = true;
                }

                var attribute = p.GetCustomAttribute<MyDataFieldAttribute>(false);
                if (attribute == null)
                {
                    entity.Properties.Add(new MyProperty
                    {
                        DataBaseColumn = p.Name,
                        PropertyName = p.Name
                    });
                }
                else
                {
                    entity.Properties.Add(new MyProperty
                    {
                        PropertyName = p.Name,
                        DataBaseColumn = string.IsNullOrWhiteSpace(attribute.ColumnName) ? p.Name : attribute.ColumnName,
                        InsertIgnore = attribute.InsertIgnore,
                        UpdateIgnore = attribute.UpdateIgnore,
                        Ignore = attribute.Ignore
                    });
                }
            }

            if (string.IsNullOrWhiteSpace(entity.EntityKeyName))
            {
                entity.EntityKeyName = "Id";
                entity.TableKeyColumn = "Id";
            }

            entity.InsertSqlStatement = BuildCommonInsertSqlStatement(entity);
            entity.UpdateSqlStatement = BuildCommonUpdateSqlStatement(entity);

            return entity;
        }

        /// <summary>
        /// 检查指定属性是否是受支持的数据类型
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool CheckPropertyType(PropertyInfo property)
        {
            string[] types = new string[]
            {
                "Byte", "SByte", "Int16", "UInt16", "Int32", "UInt32", "Int64", "UInt64", "Single", "Double", "Boolean",
                "String", "Char", "Guid", "DateTime", "Byte[]", "DateTimeOffset"
            };

            string realType = GetPropertyRealType(property);
            return types.Contains(realType);
        }

        private static string GetPropertyRealType(PropertyInfo property)
        {
            string typeName;
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeName = property.PropertyType.GetGenericArguments()[0].Name;
            }
            else
            {
                typeName = property.PropertyType.Name;
            }
            return typeName;
        }
        #endregion
    }
}
