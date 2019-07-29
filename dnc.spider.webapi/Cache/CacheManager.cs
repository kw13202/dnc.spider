using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dnc.spider.webapi
{
    public class CacheManager
    {
        public CacheManager()
        {

        }

        public static Browser browser = null;
        public static bool IsProxyJobRunning = false;
    }
}
