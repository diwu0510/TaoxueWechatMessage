using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Taoxue.Mp.Sms.Website.Extensions
{
    public static class WxNoticeServiceExtension
    {
        public static void AddWxNoticeServer(this IServiceCollection services, ILoggerFactory logger)
        {
            var provider = services.BuildServiceProvider();
            //services.AddSingleton<IWxNoticeService>(new WxNoticeService(logger));
            services.AddSingleton<IWxNoticeService>(new WxNoticeService(provider.GetService<ILoggerFactory>()));
        }
    }

    public interface IWxNoticeService
    {
        void Add(string message);
    }

    public class WxNoticeConfig
    {
        /// <summary>
        /// 时间间隔
        /// </summary>
        public int TimeInterval { get; set; }

        public int SleepCount { get; set; } = 5;
    }

    public class WxNoticeService : IWxNoticeService
    {
        private static ConcurrentQueue<string> queue;
        private static Task task;
        private static ILogger log;

        private int TimeInterval = 5000;
        private int Count = 1;
        private bool Sleeping = false;

        public WxNoticeService(ILoggerFactory logger)
        {
            log = logger.CreateLogger("WxNoticeService");
            queue = new ConcurrentQueue<string>();
            task = new TaskFactory().StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!queue.IsEmpty)
                        {
                            string val;
                            if (queue.TryDequeue(out val))
                            {
                                
                                log.LogDebug($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 获取到消息：{val}");
                            }
                            else
                            {
                                log.LogDebug("队列弹出失败");
                            }
                        }
                        else
                        {
                            log.LogDebug($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：未获取到消息");
                            Sleep();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogDebug($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{ex.Message}");
                    }
                }
            });
            log.LogDebug("通知服务开始运行");
        }

        public void Add(string id)
        {
            queue.Enqueue(id);
            Work();
        }

        private void Sleep()
        {
            Sleeping = true;
            Count++;
            Thread.Sleep(TimeInterval * Count);
        }

        private void Work()
        {
                Count = 1;
                Sleeping = false;
                Thread.Sleep(0);
        }
    }
}
