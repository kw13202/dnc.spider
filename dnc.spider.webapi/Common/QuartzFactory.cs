using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace dnc.spider.webapi
{
    /// <summary>
    /// 
    /// </summary>
    public class QuartzFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static async void StartSpider()
        {
            try
            {
                //1、通过调度工厂获得调度器
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();
                //2、开启调度器
                await scheduler.Start();
                //3、创建一个触发器
                var trigger = TriggerBuilder.Create()
#if DEBUG
                    .WithCronSchedule("0 0/1 * * * ? ")//每分钟执行一次
#else
                    .WithCronSchedule("0 0 0/8 * * ? ")//每8小时执行一次
#endif
                    .Build();
                //4、创建任务
                var jobDetail = JobBuilder.Create<SpiderJob>()
                    .WithIdentity("job", "group")
                    .Build();
                //5、将触发器和任务器绑定到调度器中
                await scheduler.ScheduleJob(jobDetail, trigger);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
