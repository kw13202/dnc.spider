using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.efcontext.Map
{
    public class SysConfigMap : IEntityTypeConfiguration<SysConfig>
    {
        public void Configure(EntityTypeBuilder<SysConfig> builder)
        {
            //表名
            builder.ToTable("SysConfig");

            //主键
            builder.HasKey(x => x.Code);

            //字段
            builder.Property(x => x.Code).HasColumnType("nvarchar(50)");
            builder.Property(x => x.Value).HasColumnType("nvarchar(50)");
        }
    }
}
