﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dnc.efcontext
{
    public class EfContext : DbContext
    {
        public EfContext()
        {

        }

        public EfContext(DbContextOptions<EfContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlite(configuration.GetConnectionString("spiderConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 通过反射注册Map
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(q => q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName) != null);
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }

        public DbSet<Goods> Goods { get; set; }
        public DbSet<HisPrice> HisPrices { get; set; }
    }
}