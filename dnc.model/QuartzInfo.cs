using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.model
{
    /// <summary>
    /// 定时任务信息
    /// </summary>
    public class QuartzInfo
    {
        /// <summary>
        /// 定时任务Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 触发器分组名
        /// </summary>
        public string TriggerGroup { get; set; }
        /// <summary>
        /// 触发器名称
        /// </summary>
        public string TriggerName { get; set; }
        /// <summary>
        /// cron表达式
        /// </summary>
        public string CronExpression { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FullClassName { get; set; }
        /// <summary>
        /// 任务分组名
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        public string Remark { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
