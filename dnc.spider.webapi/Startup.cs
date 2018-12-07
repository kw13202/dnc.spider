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

namespace dnc.spider.webapi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EfContext>(options => options.UseSqlite(Configuration.GetConnectionString("spiderConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//注册ISchedulerFactory的实例。
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory, 
            IApplicationLifetime lifetime, 
            IServiceProvider container)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<EfContext>();
                if (context.Database.EnsureCreated())
                {
                    if (!context.Goods.Any())
                    {
                        var goodList = new List<Goods>()
                        {
                            new Goods(){ GoodsCode = "7254027"},
                            new Goods(){ GoodsCode = "5008395"},
                        };
                        context.Goods.AddRange(goodList);
                        context.SaveChanges();
                    }
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();

            //var quartz = new QuartzStartup(container);
            //lifetime.ApplicationStarted.Register(quartz.Start);
            //lifetime.ApplicationStopping.Register(quartz.Stop);

        }
    }
}
