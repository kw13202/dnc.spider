using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium.Chrome;

namespace dnc.spider.helper
{
    public class ChromeDriverHelper
    {
        private readonly ChromeDriver _driver = null;

        
        //public class LoadEventArgs : EventArgs
        //{

        //}
        //public delegate void LoadEventHandler(Object sender, LoadEventArgs e);
        public event EventHandler LoadCompleted;
        protected virtual void OnCompleted(EventArgs e)
        {
            LoadCompleted?.Invoke(_driver, e);
        }

        public bool IsRuning { get; private set; } = false;

        public ChromeDriverHelper()
        {
            _driver = new ChromeDriver();
        }

        public ChromeDriverHelper(ChromeDriverOption options)
        {
            ChromeOptions opt = new ChromeOptions();
            foreach (var item in options.ChromeArguments)
            {
                opt.AddArgument(item);
            }
            opt.AddArgument("--headless");
            _driver = new ChromeDriver(options.ChromeDriverDirectory, opt);
        }

        public void GoToUrl(List<string> urls)
        {
            IsRuning = true;
            foreach (var url in urls)
            {
                _driver.Navigate().GoToUrl(url);
                OnCompleted(new EventArgs());
            }
            _driver.Quit();
            IsRuning = false;
        }



    }

    public class ChromeDriverOption
    {
        public string ChromeDriverDirectory { get; set; }
        public List<string> ChromeArguments { get; private set; } = new List<string>();
    }
}
