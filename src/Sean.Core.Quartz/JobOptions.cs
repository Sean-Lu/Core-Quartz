using System;
using Quartz;

namespace Sean.Core.Quartz
{
    public class JobOptions
    {
        /// <summary>
        /// 实现<see cref="IJob"/>接口的类
        /// </summary>
        public Type JobType { get; set; }
        /// <summary>
        /// Schedule类型
        /// </summary>
        public ScheduleType ScheduleType { get; set; }
        /// <summary>
        /// cron表达式，示例：0 0 0 * * ?（每天00:00执行1次）【仅当 <see cref="ScheduleType"/>=<see cref="ScheduleType.CronSchedule"/> 时才有效】
        /// </summary>
        public string CronExpression { get; set; }
        /// <summary>
        /// 处理简单Job的委托【仅当 <see cref="ScheduleType"/>=<see cref="ScheduleType.SimpleSchedule"/> 时才有效】
        /// </summary>
        public Action<SimpleScheduleBuilder> SimpleScheduleAction { get; set; }
        /// <summary>
        /// 当服务启动时，是否立即执行1次job
        /// </summary>
        public bool IsStartNow { get; set; }
    }
}
