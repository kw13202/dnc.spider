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

namespace dnc.spider.webapi
{
    public class InitHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly IServiceScopeFactory _scopeFactory;

        public InitHostedService(ILogger<InitHostedService> logger, IScheduler scheduler, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scheduler = scheduler;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

            var scope = _scopeFactory.CreateScope();

            #region 初始化数据库
            _logger.LogInformation("初始化数据库开始");
            try
            {
                var _context = scope.ServiceProvider.GetRequiredService<EfContext>();
                //await _context.Database.EnsureDeletedAsync();
                if (await _context.Database.EnsureCreatedAsync())
                {
                    if (!_context.QuartzInfos.Any())
                    {
                        _context.Add(new QuartzInfo
                        {
                            Guid = Guid.NewGuid().ToString(),
                            TriggerGroup = "TriggerGroup1",
                            TriggerName = "TriggerName1",
                            CronExpression = "0 0/1 * * * ? ",//每分钟执行一次
                            FullClassName = "dnc.spider.webapi.SpiderJob",
                            JobGroup = "JobGroup1",
                            JobName = "JobName1",
                        });
                        await _context.SaveChangesAsync();
                    }
                    if (!_context.Goods.Any())
                    {
                        var goodList = new List<Goods>()
                        {
                            new Goods(){ GoodsCode = "7254027"},
                            new Goods(){ GoodsCode = "5008395"},
                            new Goods(){ GoodsCode = "830486"},
                        };
                        _context.Goods.AddRange(goodList);
                        _context.SaveChanges();
                    }
                }

                _logger.LogInformation("初始化数据库结束");
            }
            catch (Exception ex)
            {
                _logger.LogError("初始化数据库出错", ex.Message);
                throw ex;
            }
            #endregion

            #region Quartz调度
            if (CacheManager.browser != null)
            {
                _logger.LogInformation("开始Quartz调度...");
                try
                {
                    // 开启调度器
                    await _scheduler.Start(stoppingToken);

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
                catch (Exception ex)
                {
                    _logger.LogError("初始化数据库出错", ex.Message);
                    throw ex;
                }
            }
            else
            {
                _logger.LogInformation("跳转Quartz调度...");
            }
            #endregion

        }
    }
}
