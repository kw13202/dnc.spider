using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using dnc.efcontext;
using dnc.model;
using Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dnc.spider.webapi
{
    public class InitHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public InitHostedService(ILogger<InitHostedService> logger, IScheduler scheduler, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scheduler = scheduler;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var usedPuppeteerSharp = _configuration.GetValue<bool>("UsedPuppeteerSharp", false);

            if (usedPuppeteerSharp)
            {
                #region 无头浏览器初始化
                try
                {
                    string downloadPath = Path.Combine(AppContext.BaseDirectory, ".local-chromium");
                    // 这句代码会自动下载无头浏览器
                    var browser = new BrowserFetcher(new BrowserFetcherOptions
                    {
                        Path = downloadPath
                    });
                    await browser.DownloadAsync(BrowserFetcher.DefaultRevision);

                    // 设置启动参数
                    CacheManager.browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError("无头浏览器初始化出错", ex.Message);
                }
                #endregion
            }

            var scope = _scopeFactory.CreateScope();

            #region EF自动迁移
            _logger.LogDebug("检查EF是否需要迁移");
            try
            {
                var _context = scope.ServiceProvider.GetRequiredService<EfContext>();
                // 判断是否有待迁移
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _logger.LogDebug("执行EF迁移");
                    _context.Database.Migrate();
                    _logger.LogDebug("执行EF迁移完毕");
                } 
                else
                {
                    _logger.LogDebug("无需执行EF迁移");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("执行EF迁移出错", ex.Message);
                throw ex;
            }
            #endregion

            #region Quartz调度
            //if (CacheManager.browser != null)
            //{
            //    _logger.LogInformation("开始Quartz调度...");
            //    try
            //    {
            //        // 开启调度器
            //        await _scheduler.Start(stoppingToken);

            //        var _context = scope.ServiceProvider.GetRequiredService<EfContext>();

            //        var list = await _context.QuartzInfos.AsNoTracking().Where(x => x.Enabled).ToListAsync();
            //        foreach (var item in list)
            //        {
            //            var jobKey = new JobKey(item.JobName, item.JobGroup);
            //            // 创建触发器
            //            var trigger = TriggerBuilder.Create()
            //                                .WithIdentity(item.TriggerName, item.TriggerGroup)
            //                                .WithCronSchedule(item.CronExpression)
            //                                .Build();

            //            // 创建任务
            //            Type type = Type.GetType(item.FullClassName);
            //            var jobDetail = JobBuilder.Create(type)
            //                                .WithIdentity(jobKey)
            //                                .Build();

            //            await _scheduler.ScheduleJob(jobDetail, trigger);

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError("Quartz调度出错", ex.Message);
            //        throw ex;
            //    }
            //}
            //else
            //{
            //    _logger.LogInformation("跳过Quartz调度...");
            //}
            _logger.LogDebug("开始Quartz调度...");
            try
            {
                // 开启调度器
                await _scheduler.Start(stoppingToken);

                var _context = scope.ServiceProvider.GetRequiredService<EfContext>();

                var list = await _context.QuartzInfos.AsNoTracking().Where(x => x.Enabled).ToListAsync();
                if(list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var jobKey = new JobKey(item.JobName, item.JobGroup);
                        // 创建触发器
                        var trigger = TriggerBuilder.Create()
                                            .WithIdentity(item.TriggerName, item.TriggerGroup)
                                            .WithCronSchedule(item.CronExpression)
                                            .Build();

                        // 创建任务
                        Type type = Type.GetType(item.FullClassName);
                        var jobDetail = JobBuilder.Create(type)
                                            .WithIdentity(jobKey)
                                            .Build();

                        await _scheduler.ScheduleJob(jobDetail, trigger);

                        _logger.LogDebug($"{ item.Remark }加入调度任务");
                    }
                }
                else
                {
                    _logger.LogDebug($"没有Quartz调度任务");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Quartz调度出错", ex.Message);
                throw ex;
            }
            #endregion

        }
    }
}
