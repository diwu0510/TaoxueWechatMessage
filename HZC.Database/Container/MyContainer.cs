using System;
using System.Collections.Concurrent;

namespace HZC.Database
{
    public class MyContainer
    {
        /// <summary>
        /// 实体及实体信息的字典
        /// </summary>
        private static ConcurrentDictionary<string, MyEntity> _dict = new ConcurrentDictionary<string, MyEntity>();

        #region 公共方法
        public static MyEntity Get(Type type)
        {
            MyEntity result;
            if (!_dict.TryGetValue(type.Name, out result))
            {
                result = MyEntityUtil.ConvertToMyEntity(type);
                _dict.TryAdd(type.Name, result);
            }
            return result;
        }
        #endregion
    }
}
