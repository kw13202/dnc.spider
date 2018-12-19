using System;
using System.Collections.Generic;
using System.Text;
using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dnc.efcontext.Map
{
    class HisPriceMap : IEntityTypeConfiguration<HisPrice>
    {
        public void Configure(EntityTypeBuilder<HisPrice> builder)
        {
            //表名
            builder.ToTable("HisPrices");

            //主键
            builder.HasKey(x => x.Id);

            //字段
            builder.Property(x => x.GoodsCode).HasColumnType("nvarchar(100)");
            builder.Property(x => x.CurPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.PlusPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.DiscountPrice).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.DiscountDesc).HasColumnType("decimal(8, 2)");
            builder.Property(x => x.SpiderTime).HasColumnType("datetime");

            //索引

            //关系
        }
    }
}
