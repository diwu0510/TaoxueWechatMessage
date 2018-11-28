using System.Collections.Generic;
using System.Linq;

namespace Taoxue.Mp.Sms.Services
{
    public static class MessageTemplateUtil
    {
        private static IEnumerable<MessageTemplateEntity> _temps;

        private static readonly object _obj = new object();

        private static void Init()
        {
            var service = new MessageTemplateService();
            _temps = service.Fetch(new MessageTemplateSearchParam { Enabled = true });
        }

        /// <summary>
        /// 所有模板
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MessageTemplateEntity> All()
        {
            lock (_obj)
            {
                if (_temps == null)
                {
                    Init();
                }
                return _temps;
            }
        }

        /// <summary>
        /// 加载模板
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        public static MessageTemplateEntity Get(int id)
        {
            return All().Where(t => t.Id == id).SingleOrDefault();
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public static void Clear()
        {
            _temps = null;
        }
    }
}
