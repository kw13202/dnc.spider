using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace dnc.spider.webapi
{
    public class QuartzStartup
    {
        private IScheduler _scheduler; // after Start, and until shutdown completes, references the scheduler object
        private readonly IServiceProvider _container;

        public QuartzStartup(IServiceProvider container)
        {
            _container = container;
        }

        // starts the scheduler, defines the jobs and the triggers
        public void Start()
        {
            if (_scheduler != null)
            {
                throw new InvalidOperationException("Already started.");
            }

            //1、通过调度工厂获得调度器
            StdSchedulerFactory factory = new StdSchedulerFactory();
            
            _scheduler = factory.GetScheduler().Result;
            _scheduler.JobFactory = new JobFactory(_container);
            //2、开启调度器
            _scheduler.Start().Wait();
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
            _scheduler.ScheduleJob(jobDetail, trigger).Wait();

        }

        // initiates shutdown of the scheduler, and waits until jobs exit gracefully (within allotted timeout)
        public void Stop()
        {
            if (_scheduler == null)
            {
                return;
            }

            // give running jobs 30 sec (for example) to stop gracefully
            if (_scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
            {
                _scheduler = null;
            }
            else
            {
                // jobs didn't exit in timely fashion - log a warning...
            }
        }
    }
}
