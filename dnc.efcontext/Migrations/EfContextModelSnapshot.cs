﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dnc.efcontext;

namespace dnc.efcontext.Migrations
{
    [DbContext(typeof(EfContext))]
    partial class EfContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("dnc.model.Goods", b =>
                {
                    b.Property<string>("GoodsCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<decimal>("CurPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<string>("DiscountDesc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("DiscountPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<string>("GoodsName")
                        .HasColumnType("nvarchar(500)");

                    b.Property<decimal?>("LowestPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime?>("LowestPriceTime")
                        .HasColumnType("datetime");

                    b.Property<decimal?>("PlusPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime?>("SpiderTime")
                        .HasColumnType("datetime");

                    b.HasKey("GoodsCode");

                    b.ToTable("Goods");
                });

            modelBuilder.Entity("dnc.model.HisPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("CurPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<string>("DiscountDesc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("DiscountPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<string>("GoodsCode")
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal?>("PlusPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime>("SpiderTime")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("HisPrices");
                });

            modelBuilder.Entity("dnc.model.Proxy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IP")
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool?>("IsValid");

                    b.Property<int>("Port");

                    b.HasKey("Id");

                    b.HasIndex("IsValid");

                    b.ToTable("Proxy");
                });

            modelBuilder.Entity("dnc.model.QuartzInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CronExpression")
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("Enabled");

                    b.Property<string>("FullClassName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("JobGroup")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("JobName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Remark");

                    b.Property<string>("TriggerGroup")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TriggerName")
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("QuartzInfos");
                });

            modelBuilder.Entity("dnc.model.SysConfig", b =>
                {
                    b.Property<string>("Code")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Code");

                    b.ToTable("SysConfig");
                });
#pragma warning restore 612, 618
        }
    }
}
