using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using dnc.efcontext;
using dnc.model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Quartz.Spi;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Rewrite;

namespace dnc.spider.webapi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // 添加日志Service
            services.AddLogging();

            // 设置efcore连接字符串
            //services.AddDbContext<EfContext>(options => options.UseSqlite(Configuration.GetConnectionString("spiderConnection")));
            services.AddDbContext<EfContext>(options => options.UseSqlServer(Configuration.GetConnectionString("mssqlConn")));

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton(provider =>
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                var scheduler = factory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();

                scheduler.JobFactory = provider.GetService<IJobFactory>();

                return scheduler;
            });
            services.AddSingleton<SpiderJob, SpiderJob>();
            services.AddSingleton<ProxyJob, ProxyJob>();

            // 添加单例IOC
            services.AddTransient<CacheManager>();

            // 允许跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()    // 允许所有来源
                           .AllowAnyHeader()    // 允许所有头
                           .AllowAnyMethod();   // 允许所有方法
                });
            });


            // 设置MVC版本号
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // 设置Swagger
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info()
                {
                    Title = "爬虫API说明文档",
                    Description = "ASP.NET Core Web API 例子",
                    Version = "v1"
                });
                //为Swagger的JSON和UI设置XML注释
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            // 添加HttpClientFactory
            services.AddHttpClient("proxy", x =>
            {
                x.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                x.DefaultRequestHeaders.Add("Accept-Encoding", "");// value gzip, deflate, br
                x.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
                x.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                x.DefaultRequestHeaders.Add("Connection", "keep-alive");
                x.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:68.0) Gecko/20100101 Firefox/68.0");
                x.Timeout = TimeSpan.FromSeconds(20);
            });

            // 添加自定义的HostedService
            services.AddHostedService<InitHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // 使用静态文件
            app.UseStaticFiles();
            // 使用Swagger
            app.UseSwagger();
            // 使用SwaggerUI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //c.RoutePrefix = string.Empty;
            });
            // 使用跨域，必须在UseMvc之前
            app.UseCors();
            //使用NLog作为日志记录工具
            loggerFactory.AddNLog();
            //引入Nlog配置文件
            env.ConfigureNLog("nlog.config");

            // 设置重定向
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            option.AddRedirect("^index.html$", "swagger");
            app.UseRewriter(option);

            // 使用MVC
            app.UseMvc();
        }
    }
}
