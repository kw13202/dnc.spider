using System;
using System.Collections.Generic;
using System.Text;
using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dnc.efcontext.Map
{
    public class GoodsMap : IEntityTypeConfiguration<Goods>
    {
        public void Configure(EntityTypeBuilder<Goods> builder)
        {
            //表名
            builder.ToTable("Goods");

            //主键
            builder.HasKey(x => x.GoodsCode);

            //字段
            builder.Property(x => x.GoodsCode).HasColumnType("nvarchar(100)");
            builder.Property(x => x.GoodsName).HasColumnType("nvarchar(500)");
            builder.Property(x => x.LowestPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.LowestPriceTime).HasColumnType("datetime");
            builder.Property(x => x.CurPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.PlusPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.DiscountPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.DiscountDesc).HasColumnType("nvarchar(max)");
            builder.Property(x => x.SpiderTime).HasColumnType("datetime");
            builder.Property(x => x.CreateTime).HasColumnType("datetime");

            //索引

            //关系
            //builder.HasMany(x => x.HisPrices).WithOne(x => x.Good).HasForeignKey(x => x.GoodsCode);
        }
    }
}
