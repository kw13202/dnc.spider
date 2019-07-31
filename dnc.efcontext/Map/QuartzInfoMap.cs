using dnc.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.efcontext.Map
{
    public class QuartzInfoMap : IEntityTypeConfiguration<QuartzInfo>
    {
        public void Configure(EntityTypeBuilder<QuartzInfo> builder)
        {
            //表名
            builder.ToTable("QuartzInfos");

            //主键
            builder.HasKey(x => x.Id);

            //字段
            builder.Property(x => x.TriggerGroup).HasColumnType("nvarchar(100)");
            builder.Property(x => x.TriggerName).HasColumnType("nvarchar(100)");
            builder.Property(x => x.CronExpression).HasColumnType("nvarchar(200)");
            builder.Property(x => x.FullClassName).HasColumnType("nvarchar(100)");
            builder.Property(x => x.JobGroup).HasColumnType("nvarchar(100)");
            builder.Property(x => x.JobName).HasColumnType("nvarchar(100)");

            //索引

            //关系

        }
    }
}
