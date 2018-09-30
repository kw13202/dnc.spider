using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dnc.spider.helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Quartz;

namespace dnc.spider.webapi
{
    /// <summary>
    /// 
    /// </summary>
    public class SpiderJob : IJob
    {
        private ChromeDriverHelper chromeHelper = new ChromeDriverHelper(new ChromeDriverOption()
        {
            ChromeDriverDirectory = AppDomain.CurrentDomain.BaseDirectory,
        });
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                //遇到最坑的地方：2.4.2的ChromeDriver必须匹配69以上的chrome，低版本67直接不行
                //var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
                try
                {

                    if(chromeHelper.IsRuning)
                        return;

                    var links = new List<string>()
                    {
                        "https://item.jd.com/7254027.html",
                        "https://item.jd.com/5008395.html"
                    };

                    chromeHelper.LoadCompleted += (sender, args) =>
                    {
                        var driver = sender as ChromeDriver;
                        var title = driver.FindElement(By.CssSelector(".sku-name"));
                        var price = driver.FindElement(By.CssSelector(".p-price > .price"));
                        var plusPrice = driver.FindElement(By.CssSelector(".p-price-plus > .price"));

                        Console.WriteLine($"Title:{title?.Text}");
                        Console.WriteLine($"Price:{price?.Text}");
                        Console.WriteLine($"PlusPrice:{plusPrice?.Text}");
                        Console.WriteLine("");

                    };
                    chromeHelper.GoToUrl(links);

                    //foreach (var item in links)
                    //{
                    //    driver.Navigate().GoToUrl(item);

                    //    var title = driver.FindElement(By.CssSelector(".sku-name"));
                    //    var price = driver.FindElement(By.CssSelector(".p-price > .price"));
                    //    var plusPrice = driver.FindElement(By.CssSelector(".p-price-plus > .price"));


                    //    Console.WriteLine($"Title:{title?.Text}");
                    //    Console.WriteLine($"Price:{price?.Text}");
                    //    Console.WriteLine($"PlusPrice:{plusPrice?.Text}");
                    //    Console.WriteLine("");
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    //driver.Quit();
                }
            });

        }
    }
}
