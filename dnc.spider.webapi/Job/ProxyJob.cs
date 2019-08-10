using dnc.efcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Threading;
using dnc.model;

namespace dnc.spider.webapi
{
    /// <summary>
    /// 代理类定时任务
    /// </summary>
    public class ProxyJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;


        public ProxyJob(ILogger<ProxyJob> logger, IConfiguration config, IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _config = config;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            if (CacheManager.IsProxyJobRunning)
            {
                _logger.LogInformation(string.Format("[{0:yyyy-MM-dd hh:mm:ss:ffffff}]ProxyJob任务还在执行，请等待执行完毕！", DateTime.Now));
                return;
            }
            else
            {
                _logger.LogInformation(string.Format("[{0:yyyy-MM-dd hh:mm:ss:ffffff}]ProxyJob任务执行！", DateTime.Now));
            }
            CacheManager.IsProxyJobRunning = true;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<EfContext>();
                try
                {
                    string url = "https://www.kuaidaili.com/free/";
                    // 获取现有代理
                    var proxyList = await _context.Proxy.ToListAsync();
                    // 访问页面
                    var client = _httpClientFactory.CreateClient("proxy");
                    var rsp = await client.GetAsync(url);
                    var content = await rsp.Content.ReadAsStringAsync();
                    if (rsp.IsSuccessStatusCode)
                    {
                        var parser = new HtmlParserHelper(content);
                        var pagesList = parser.GetTextList("#listnav a");
                        var pages = pagesList.Last();
                        if (Int32.TryParse(pages, out int result))
                        {
                            var taskQueue = new ConcurrentQueue<int>();
                            for (int i = 1; i <= result; i++)
                            {
                                taskQueue.Enqueue(i);
                            }
                            
                            if (taskQueue.Count > 0)
                            {
                                while (taskQueue.Count > 0)
                                {
                                    if (taskQueue.TryDequeue(out int page))
                                    {
                                        _logger.LogDebug($"ThreadId { Thread.CurrentThread.ManagedThreadId }, Page { page }");
                                        string addr = $"https://www.kuaidaili.com/free/inha/{page}/";
                                        var tempRsp = await client.GetAsync(addr);
                                        if (tempRsp.IsSuccessStatusCode)
                                        {
                                            var text = await tempRsp.Content.ReadAsStringAsync();
                                            var htmlParser = new HtmlParserHelper(content);
                                            var trList = parser.QuerySelectorAll("#list tbody tr");
                                            List<Proxy> addList = new List<Proxy>();
                                            foreach (var item in trList)
                                            {
                                                var tdList = item.QuerySelectorAll("td");
                                                string ip = tdList[0].TextContent.Trim();
                                                string port = tdList[1].TextContent.Trim();
                                                var model = proxyList.FirstOrDefault(x => x.IP.Equals(ip) && x.Port == Convert.ToInt32(port));
                                                if (model == null)
                                                {
                                                    addList.Add(new Proxy
                                                    {
                                                        IP = ip,
                                                        Port = Convert.ToInt32(port)
                                                    });
                                                }
                                            }
                                            if (addList.Count > 0)
                                            {
                                                _context.Proxy.AddRange(addList);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            _logger.LogDebug($"网站返回错误");
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(10));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError($"code:{ rsp.StatusCode },content:{ content }");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("[{0:yyyy-MM-dd hh:mm:ss:ffffff}]ProxyJob任务出错！", ex);
                    throw ex;
                }
                finally
                {
                    CacheManager.IsProxyJobRunning = false;
                }
            }

            
            _logger.LogInformation(string.Format("[{0:yyyy-MM-dd hh:mm:ss:ffffff}]ProxyJob任务执行完毕！", DateTime.Now));
        }
    }
}
