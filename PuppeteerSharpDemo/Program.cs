using System;
using System.Threading.Tasks;
// 无头浏览器API
using PuppeteerSharp;
// Html解析
using AngleSharp;
using System.IO;
using System.Text;

namespace PuppeteerSharpDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 这句代码会自动下载无头浏览器
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            // 设置启动参数
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            // 新建页面
            var page = await browser.NewPageAsync();
            // 页面访问
            await page.GoToAsync("https://item.jd.com/100002293180.html");
            // 获取访问内容
            var htmlString = await page.GetContentAsync();
            // 保存
            string basePath = Directory.GetCurrentDirectory();
            using (FileStream fs = new FileStream($"{basePath}\\jd_html.txt", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                byte[] content = Encoding.UTF8.GetBytes(htmlString);
                await fs.WriteAsync(content, 0, content.Length);
            }

            Console.WriteLine(htmlString);

            Console.ReadKey();
        }
    }
}
