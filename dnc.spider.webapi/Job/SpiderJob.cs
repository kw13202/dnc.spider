using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dnc.efcontext;
using dnc.spider.helper;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Quartz;
using SQLitePCL;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Text;
using dnc.model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using AngleSharp.Html.Parser;

namespace dnc.spider.webapi
{
    /// <summary>
    /// 
    /// </summary>
    public class SpiderJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;

        public SpiderJob(ILogger<SpiderJob> logger, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _config = config;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(string.Format("[{0:yyyy-MM-dd hh:mm:ss:ffffff}]任务执行！", DateTime.Now));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<EfContext>();
                // 新建页面
                var page = await CacheManager.browser.NewPageAsync();

                try
                {
                    var list = await _context.Goods.ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            // 页面访问
                            await page.GoToAsync($"https://item.jd.com/{item.GoodsCode}.html");
                            // 获取访问内容
                            var htmlString = await page.GetContentAsync();
                            // 保存
                            string basePath = AppContext.BaseDirectory;
                            using (FileStream fs = new FileStream($"{basePath}{Path.DirectorySeparatorChar}jd_{item.GoodsCode}.txt", FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                byte[] content = Encoding.UTF8.GetBytes(htmlString);
                                await fs.WriteAsync(content, 0, content.Length);
                            }

                            string text = await File.ReadAllTextAsync($"{basePath}{Path.DirectorySeparatorChar}jd_{item.GoodsCode}.txt", Encoding.UTF8);

                            var parser = new HtmlParserHelper(htmlString);
                            string goodsName = parser.GetText(".sku-name");
                            decimal? curPrice = parser.GetDecimal(".p-price > .price");
                            decimal? plusPrice = parser.GetDecimal(".p-price-plus > .price");
                            List<string> discountTypeList = parser.GetTextList(".J-prom .hl_red_bg");
                            List<string> discountDescList = parser.GetTextList(".J-prom .hl_red");
                            decimal? discoutPrice = null;

                            if (discountTypeList != null && discountTypeList.Count > 0 && discountDescList != null && discountDescList.Count > 0)
                            {
                                if (discountTypeList.Count == discountDescList.Count)
                                {
                                    int len = discountTypeList.Count;
                                    for (int i = 0; i < len; i++)
                                    {
                                        string discountType = discountTypeList[i];
                                        string discountDesc = discountDescList[i];

                                        switch (discountType)
                                        {
                                            case "满减":
                                                #region 满减
                                                {
                                                    Regex reg = new Regex(@"满\s*([\d\.]+)\s*元减\s*([\d\.]+)\s*元*");
                                                    var matchs = reg.Matches(discountDesc);
                                                    foreach (Match match in matchs)
                                                    {
                                                        decimal num1 = Convert.ToDecimal(match.Groups[1].Value);
                                                        decimal num2 = Convert.ToDecimal(match.Groups[2].Value);
                                                        if (curPrice >= num1)
                                                        {
                                                            discoutPrice = curPrice - num2;
                                                            //sb.AppendLine($"{match.Groups[0].Value}({curPrice}-{num2}={discoutPrice})");
                                                        }
                                                        else
                                                        {
                                                            //sb.AppendLine($"{match.Groups[0].Value}(需凑单)");
                                                        }
                                                    }
                                                }
                                                #endregion
                                                break;
                                            default:
                                                #region 其他
                                                {
                                                    _logger.LogWarning("未处理的优惠类型");
                                                    using (FileStream fs = new FileStream($"{basePath}{Path.DirectorySeparatorChar}jd_{item.GoodsCode}.txt", FileMode.Create, FileAccess.Write, FileShare.Write))
                                                    {
                                                        byte[] content = Encoding.UTF8.GetBytes(htmlString);
                                                        await fs.WriteAsync(content, 0, content.Length);
                                                    }
                                                }
                                                #endregion
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("优惠类型与描述不匹配");
                                }
                            } 
                            else
                            {
                                _logger.LogDebug("没有优惠");
                            }


                            item.GoodsName = goodsName;
                            item.CurPrice = curPrice.HasValue ? curPrice.Value : 0;
                            item.PlusPrice = plusPrice.HasValue ? plusPrice.Value : 0;
                            item.SpiderTime = DateTime.Now;


                            var hisPrice = await _context.HisPrices.OrderByDescending(x => x.SpiderTime).FirstOrDefaultAsync(x => item.GoodsCode.Equals(x.GoodsCode));
                            if (hisPrice != null)
                            {
                                hisPrice.CurPrice = curPrice.HasValue ? curPrice.Value : 0;
                                hisPrice.PlusPrice = plusPrice.HasValue ? plusPrice.Value : 0;
                                hisPrice.SpiderTime = DateTime.Now;
                            }
                            else
                            {
                                await _context.HisPrices.AddAsync(new HisPrice()
                                {
                                    GoodsCode = item.GoodsCode,
                                    CurPrice = curPrice.HasValue ? curPrice.Value : 0,
                                    PlusPrice = plusPrice.HasValue ? plusPrice.Value : 0,
                                    SpiderTime = DateTime.Now,
                                });
                            }

                            await _context.SaveChangesAsync();

                            //if (!string.IsNullOrWhiteSpace(discountType))
                            //{
                            //    switch (discountType)
                            //    {
                            //        case "满减":
                            //            {
                            //                Regex reg = new Regex(@"满\s*([\d\.]+)\s*元减\s*([\d\.]+)\s*元*");
                            //                var matchs = reg.Matches(discountDesc);
                            //                foreach (Match match in matchs)
                            //                {
                            //                    decimal num1 = Convert.ToDecimal(match.Groups[1].Value);
                            //                    decimal num2 = Convert.ToDecimal(match.Groups[2].Value);
                            //                    if (curPrice >= num1)
                            //                    {
                            //                        discoutPrice = curPrice - num2;
                            //                        //sb.AppendLine($"{match.Groups[0].Value}({curPrice}-{num2}={discoutPrice})");
                            //                    }
                            //                    else
                            //                    {
                            //                        //sb.AppendLine($"{match.Groups[0].Value}(需凑单)");
                            //                    }
                            //                }
                            //            }
                            //            break;
                            //    }
                            //}


                            _logger.LogDebug(goodsName);
                            _logger.LogDebug($"goodsName:{goodsName},curPrice:{(curPrice.HasValue ? curPrice.Value : 0)},plusPrice:{(plusPrice.HasValue ? plusPrice.Value : 0)}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("", ex);
                    throw;
                }
                finally
                {
                    if (page != null)
                        await page.CloseAsync();
                }
            }

        }

    }
}
