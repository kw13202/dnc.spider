using dnc.efcontext;
using dnc.model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dnc.spider.webapi
{
    public class DBHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public DBHostedService(ILogger<DBHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("初始化数据库开始");

            using (var scope = _scopeFactory.CreateScope())
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
                            JobGroup= "JobGroup1",
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
            }

            _logger.LogInformation("初始化数据库结束");
        }
    }
}
