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
            services.AddDbContext<EfContext>(options => options.UseSqlite(Configuration.GetConnectionString("spiderConnection")));

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton(provider =>
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                var scheduler = factory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();

                scheduler.JobFactory = provider.GetService<IJobFactory>();

                return scheduler;
            });
            // 添加自定义的HostedService
            services.AddHostedService<InitHostedService>();
            services.AddHostedService<DBHostedService>();
            services.AddHostedService<QuartzHostedService>();
            services.AddSingleton<SpiderJob, SpiderJob>();


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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            });
            // 使用MVC
            app.UseMvc();
        }
    }
}
