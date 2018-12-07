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
            builder.Property(x => x.GoodsCode).HasMaxLength(100);
            builder.Property(x => x.GoodsName).HasMaxLength(500);
            builder.Property(x => x.LowestPrice);
            builder.Property(x => x.LowestPriceTime);
            builder.Property(x => x.CurPrice);
            builder.Property(x => x.PlusPrice);
            builder.Property(x => x.DiscountPrice);
            builder.Property(x => x.DiscountDesc);
            builder.Property(x => x.SpiderTime);
            builder.Property(x => x.CreateTime);

            //索引

            //关系
            //builder.HasMany(x => x.HisPrices).WithOne(x => x.Good).HasForeignKey(x => x.GoodsCode);
        }
    }
}
