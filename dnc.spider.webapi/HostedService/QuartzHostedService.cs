using dnc.efcontext;
using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dnc.spider.webapi
{
    public class QuartzHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly IServiceScopeFactory _scopeFactory;

        public QuartzHostedService(ILogger<QuartzHostedService> logger, IScheduler scheduler, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scheduler = scheduler;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("开始Quartz调度...");

            // 开启调度器
            await _scheduler.Start(stoppingToken);

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<EfContext>();

                var list = await _context.QuartzInfos.AsNoTracking().ToListAsync();
                foreach (var item in list)
                {
                    // 创建触发器
                    var trigger = TriggerBuilder.Create()
                                        .WithIdentity(item.TriggerName, item.TriggerGroup)
                                        .WithCronSchedule(item.CronExpression)
                                        .Build();

                    // 创建任务
                    Type type = Type.GetType(item.FullClassName);
                    var jobDetail = JobBuilder.Create(type)
                                        .WithIdentity(item.JobName, item.JobGroup)
                                        .Build();

                    await _scheduler.ScheduleJob(jobDetail, trigger);
                }
            }
        }
    }
}
