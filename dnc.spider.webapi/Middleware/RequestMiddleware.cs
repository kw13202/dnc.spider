using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dnc.spider.webapi
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public RequestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestMiddleware>();
        }

        public Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation($"Path:{ httpContext.Request.Path }");
            _logger.LogInformation($"Client Ip:{httpContext.Connection.RemoteIpAddress.ToString()}");
            return _next(httpContext);
        }
    }
}
