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

namespace dnc.spider.webapi
{
    /// <summary>
    /// 
    /// </summary>
    public class SpiderJob : IJob
    {
        //private readonly EfContext _context;

        //public SpiderJob(EfContext context)
        //{
        //    _context = context;
        //}


        private readonly ChromeDriverHelper chromeHelper = new ChromeDriverHelper(new ChromeDriverOption()
        {
            ChromeDriverDirectory = AppDomain.CurrentDomain.BaseDirectory,
        });
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
            var ctx = new EfContext();
            try
            {
                var list = await ctx.Goods.AsNoTracking().ToListAsync();
                
                if (list != null && list.Count > 0)
                {
                    foreach(var item in list)
                    {
                        driver.Navigate().GoToUrl($"http://item.jd.com/{item.GoodsCode}.html");

                        StringBuilder sb = new StringBuilder();
                        string goodsName = GetElementValue<string>(driver.FindElements(By.CssSelector(".sku-name")));
                        decimal curPrice = GetElementValue<decimal>(driver.FindElements(By.CssSelector(".p-price > .price")));
                        Nullable<decimal> plusPrice = GetDecimalElementValue(driver.FindElements(By.CssSelector(".p-price-plus > .price")));
                        string discountType = GetElementValue<string>(driver.FindElements(By.CssSelector(".J-prom .hl_red_bg")));
                        string discountDesc = GetElementValue<string>(driver.FindElements(By.CssSelector(".J-prom .hl_red")));
                        decimal? discoutPrice = null;

                        switch (discountType)
                        {
                            case "满减":
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
                                            sb.AppendLine($"{match.Groups[0].Value}({curPrice}-{num2}={discoutPrice})");
                                        } 
                                        else
                                        {
                                            sb.AppendLine($"{match.Groups[0].Value}(需凑单)");
                                        }
                                    }
                                }
                                break;
                        }
                        // 判断最低价
                        decimal min = curPrice;
                        if (plusPrice.HasValue)
                        {
                            min = Math.Min(curPrice, plusPrice.Value);
                        }
                        if (discoutPrice.HasValue)
                        {
                            min = Math.Min(min, discoutPrice.Value);
                        }
                        if (item.LowestPrice.HasValue)
                        {
                            if (item.LowestPrice.Value <= min)
                            {
                                item.LowestPrice = min;
                                item.LowestPriceTime = DateTime.Now;
                            }
                        }
                        else
                        {
                            item.LowestPrice = min;
                            item.LowestPriceTime = DateTime.Now;
                        }

                        item.GoodsName = goodsName;
                        item.CurPrice = curPrice;
                        item.PlusPrice = plusPrice;
                        item.DiscountPrice = discoutPrice;
                        item.SpiderTime = DateTime.Now;
                        ctx.Goods.Attach(item);
                        ctx.Entry(item).State = EntityState.Modified;

                        

                        var hisPrice = await ctx.HisPrices.FirstOrDefaultAsync(x => x.GoodsCode == item.GoodsCode);
                        if (hisPrice == null)
                        {
                            ctx.HisPrices.Add(new model.HisPrice()
                            {
                                GoodsCode = item.GoodsCode,
                                CurPrice = curPrice,
                                PlusPrice = plusPrice,
                                DiscountPrice = discoutPrice,
                                DiscountDesc = discountDesc,
                                SpiderTime = item.SpiderTime
                            });
                        }
                        else
                        {
                            if (hisPrice.CurPrice != curPrice || hisPrice.PlusPrice != plusPrice || hisPrice.DiscountPrice != discoutPrice)
                            {
                                ctx.HisPrices.Add(new model.HisPrice()
                                {
                                    GoodsCode = item.GoodsCode,
                                    CurPrice = curPrice,
                                    PlusPrice = plusPrice,
                                    DiscountPrice = discoutPrice,
                                    DiscountDesc = discountDesc,
                                    SpiderTime = item.SpiderTime
                                });
                            }
                        }
                        await ctx.SaveChangesAsync();

                        //var titleList = driver.FindElements(By.CssSelector(".sku-name"));
                        //var price = driver.FindElement(By.CssSelector(".p-price > .price"));
                        //var plusPrice = driver.FindElement(By.CssSelector(".p-price-plus > .price"));

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                if (ctx != null)
                    ctx.Dispose();
                if (driver != null)
                    driver.Quit();
            }

            //return Task.Run(async () =>
            //{
            //    //遇到最坑的地方：2.4.2的ChromeDriver必须匹配69以上的chrome，低版本67直接不行
            //    //var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
            //    try
            //    {

            //        if(chromeHelper.IsRuning)
            //            return;

            //        //var list = await _context.Goods.AsNoTracking().ToListAsync();
            //        //if (list != null && list.Count > 0)
            //        //{

            //        //}

            //        //var links = new List<string>()
            //        //{
            //        //    "https://item.jd.com/7254027.html",
            //        //    "https://item.jd.com/5008395.html"
            //        //};

            //        //chromeHelper.LoadCompleted += (sender, args) =>
            //        //{
            //        //    var driver = sender as ChromeDriver;
            //        //    var title = driver.FindElement(By.CssSelector(".sku-name"));
            //        //    var price = driver.FindElement(By.CssSelector(".p-price > .price"));
            //        //    var plusPrice = driver.FindElement(By.CssSelector(".p-price-plus > .price"));

            //        //    Console.WriteLine($"Title:{title?.Text}");
            //        //    Console.WriteLine($"Price:{price?.Text}");
            //        //    Console.WriteLine($"PlusPrice:{plusPrice?.Text}");
            //        //    Console.WriteLine("");

            //        //};
            //        //chromeHelper.GoToUrl(links);

            //        //foreach (var item in links)
            //        //{
            //        //    driver.Navigate().GoToUrl(item);

            //        //    var title = driver.FindElement(By.CssSelector(".sku-name"));
            //        //    var price = driver.FindElement(By.CssSelector(".p-price > .price"));
            //        //    var plusPrice = driver.FindElement(By.CssSelector(".p-price-plus > .price"));


            //        //    Console.WriteLine($"Title:{title?.Text}");
            //        //    Console.WriteLine($"Price:{price?.Text}");
            //        //    Console.WriteLine($"PlusPrice:{plusPrice?.Text}");
            //        //    Console.WriteLine("");
            //        //}
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
            //    finally
            //    {
            //        //driver.Quit();
            //    }
            //});

        }


        private T GetElementValue<T>(IReadOnlyCollection<IWebElement> list)
        {
            if (list != null  && list.Count > 0)
            {
                var ele = list.FirstOrDefault();
                if (ele != null)
                {
                    return (T)Convert.ChangeType(ele.Text, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }

        public decimal? GetDecimalElementValue(IReadOnlyCollection<IWebElement> list)
        {
            if (list != null && list.Count > 0)
            {
                var ele = list.FirstOrDefault();
                if (ele != null)
                {
                    var txt = ele.Text;
                    // 有时会有特殊符号在里面

                    if (string.IsNullOrWhiteSpace(txt))
                    {
                        return null;
                    }
                    else
                    {
                        return MatchHelper.getDecimalFirstOrDefault(txt);
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
