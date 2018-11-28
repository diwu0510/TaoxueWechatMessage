using System.Collections.Generic;
using System.Linq;

namespace Taoxue.Mp.Sms.Services
{
    public static class PlatUtil
    {
        private static IEnumerable<PlatEntity> _plats;

        private static readonly object _lock = new object();

        private static void Init()
        {
            var service = new PlatService();
            _plats = service.Fetch();
        }

        /// <summary>
        /// 获取所有第三方平台
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PlatEntity> All()
        {
            lock (_lock)
            {
                if (_plats == null)
                {
                    Init();
                }
                return _plats;
            }
        }

        /// <summary>
        /// 加载第三方平台
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PlatEntity Get(int id)
        {
            return All().Where(p => p.Id == id).SingleOrDefault();
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public static void Clear()
        {
            _plats = null;
        }
    }
}
