using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.efcontext.Map
{
    public class ProxyMap : IEntityTypeConfiguration<Proxy>
    {
        public void Configure(EntityTypeBuilder<Proxy> builder)
        {
            //表名
            builder.ToTable("Proxy");

            //主键
            builder.HasKey(x => x.Id);

            //字段
            builder.Property(x => x.IP).HasColumnType("nvarchar(50)");
            builder.Property(x => x.Port);

            //索引
            builder.HasIndex(x => x.IsValid);
        }
    }
}
