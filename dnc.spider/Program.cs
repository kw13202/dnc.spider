using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dnc.spider.helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace dnc.spider
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = @"满200元减20元，满300元减30元，满500元减50元";
            Regex reg = new Regex(@"满\s*([\d\.]+)\s*元减\s*([\d\.]+)\s*元*");
            var match = reg.Matches(text);
            foreach(Match item in match)
            {
                foreach (Group sub in item.Groups)
                {
                    Console.WriteLine(sub.Value);
                }
                
            }

            string text2 = @"￥339.00";
            decimal a = MatchHelper.getDecimalFirstOrDefault(text2);

            //遇到最坑的地方：2.4.2的ChromeDriver必须匹配69以上的chrome，低版本67直接不行
            //var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
            //try
            //{
            //    var links = new List<string>()
            //    {
            //        "http://item.jd.com/7254027.html",
            //        "http://item.jd.com/5008395.html"
            //    };

            //    foreach (var item in links)
            //    {
            //        driver.Navigate().GoToUrl(item);

            //        var title = driver.FindElement(By.CssSelector(".sku-name"));
            //        var price = driver.FindElement(By.CssSelector(".p-price > .price"));
            //        var plusPrice = driver.FindElement(By.CssSelector(".p-price-plus > .price"));


            //        Console.WriteLine($"Title:{title?.Text}");
            //        Console.WriteLine($"Price:{price?.Text}");
            //        Console.WriteLine($"PlusPrice:{plusPrice?.Text}");
            //        Console.WriteLine("");
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
            //finally
            //{
            //    driver.Quit();
            //}

            Console.Read();
        }
    }
}
