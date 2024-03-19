using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz.Simpl;
#if !NETFRAMEWORK
using QuartzRemoteScheduler.Server;
#endif

namespace Sean.Core.Quartz
{
    public class JobManager
    {
        public static IJobFactory DefaultJobFactory { get; set; }

        public IScheduler Scheduler => _scheduler;

        private readonly ISchedulerFactory _schedulerFactory;// 调度器工厂
        private readonly IScheduler _scheduler;// 调度器

        public JobManager(Action<RemoteSchedulerServerOptions> remoteSchedulerConfig = null)
        {
            var options = new RemoteSchedulerServerOptions();
            remoteSchedulerConfig?.Invoke(options);

#if NETFRAMEWORK
            // https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#remoting-server-and-client
            // WARNING: Remoting only works with .NET Full Framework. It's also considered unsafe.

            if (!options.EnableRemoteScheduler)
            {
                _schedulerFactory = new StdSchedulerFactory();
            }
            else
            {
                var properties = new NameValueCollection
                {
                    ["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz",// Required
                    ["quartz.scheduler.exporter.port"] = options.Port.ToString(),// Required
                    ["quartz.scheduler.exporter.bindName"] = options.BindName,
                    ["quartz.scheduler.exporter.channelType"] = options.ChannelType
                };
                _schedulerFactory = new StdSchedulerFactory(properties);
            }
#else
            // https://github.com/kaaja-h/QuartzRemoteScheduler => Plugin for quartz.net scheduler for enablign remote scheduler control

            if (!options.EnableRemoteScheduler)
            {
                _schedulerFactory = new StdSchedulerFactory();
            }
            else
            {
                var conf = new NameValueCollection
                {
                    ["quartz.plugin.remoteScheduler.type"] = typeof(RemoteSchedulerServerPlugin).AssemblyQualifiedName,
                    ["quartz.plugin.remoteScheduler.address"] = "0.0.0.0",
                    ["quartz.plugin.remoteScheduler.port"] = options.Port.ToString(),
                    ["quartz.plugin.remoteScheduler.enableNegotiateStream"] = "false"
                };
                _schedulerFactory = new StdSchedulerFactory(conf);
            }
#endif

            _scheduler = _schedulerFactory.GetScheduler().Result;
            if (DefaultJobFactory != null)
            {
                _scheduler.JobFactory = DefaultJobFactory;
            }
        }

#if NETFRAMEWORK
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxyAddress">服务端代理地址，示例：tcp://127.0.0.1:9192/QuartzScheduler</param>
        /// <param name="useProxyFactory"></param>
        /// <returns></returns>
        public static async Task<IScheduler> GetRemoteScheduler(string proxyAddress, bool useProxyFactory = false)
        {
            if (string.IsNullOrWhiteSpace(proxyAddress))
            {
                return null;
            }

            if (useProxyFactory)
            {
                var remotingSchedulerProxyFactory = new RemotingSchedulerProxyFactory
                {
                    Address = proxyAddress
                };
                return new RemoteScheduler(null, remotingSchedulerProxyFactory);
            }

            var properties = new NameValueCollection
            {
                //["quartz.scheduler.instanceName"] = "schedMaintenanceService",
                ["quartz.scheduler.proxy"] = "true",
                ["quartz.scheduler.proxy.address"] = proxyAddress
            };
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            return await sf.GetScheduler();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxyAddress">服务端代理地址，示例：tcp://127.0.0.1:9192/QuartzScheduler</param>
        /// <returns></returns>
        public static IRemotableQuartzScheduler GetRemoteSchedulerProxy(string proxyAddress)
        {
            if (string.IsNullOrWhiteSpace(proxyAddress))
            {
                return null;
            }

            var remotingSchedulerProxyFactory = new RemotingSchedulerProxyFactory
            {
                Address = proxyAddress
            };
            return remotingSchedulerProxyFactory.GetProxy();
        }
#endif

        /// <summary>
        /// 添加定时Job（<see cref="ScheduleType.CronSchedule"/>）
        /// </summary>
        /// <param name="jobType">job</param>
        /// <param name="cronExpression">cron表达式</param>
        /// <param name="isStartNow">当服务启动时，是否立即执行1遍job</param>
        /// <returns></returns>
        public async Task ScheduleJob(Type jobType, string cronExpression, bool isStartNow = false)
        {
            await ScheduleJob(new JobOptions
            {
                JobType = jobType,
                ScheduleType = ScheduleType.CronSchedule,
                CronExpression = cronExpression,
                IsStartNow = isStartNow
            });
        }
        /// <summary>
        /// 添加定时Job（<see cref="ScheduleType.SimpleSchedule"/>）
        /// </summary>
        /// <param name="jobType">job</param>
        /// <param name="action"></param>
        /// <param name="isStartNow">当服务启动时，是否立即执行1遍job</param>
        /// <returns></returns>
        public async Task ScheduleJob(Type jobType, Action<SimpleScheduleBuilder> action, bool isStartNow = false)
        {
            await ScheduleJob(new JobOptions
            {
                JobType = jobType,
                ScheduleType = ScheduleType.SimpleSchedule,
                SimpleScheduleAction = action,
                IsStartNow = isStartNow
            });
        }
        /// <summary>
        /// 添加定时Job
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ScheduleJob(JobOptions options)
        {
            if (options?.JobType == null)
            {
                return;
            }

            var jobName = options.JobType.Name;
            var jobGroup = $"{jobName}Group";
            var triggerName = $"{jobName}Trigger";

            IJobDetail jobDetail = JobBuilder.Create(options.JobType)
                .WithIdentity(jobName, jobGroup)
                .Build();

            TriggerBuilder triggerBuilder;
            switch (options.ScheduleType)
            {
                case ScheduleType.CronSchedule:
                    if (string.IsNullOrWhiteSpace(options.CronExpression))
                    {
                        throw new ArgumentNullException(nameof(options.CronExpression));
                    }
                    triggerBuilder = TriggerBuilder.Create().WithIdentity(triggerName, jobGroup).WithCronSchedule(options.CronExpression);
                    break;
                case ScheduleType.SimpleSchedule:
                    if (options.SimpleScheduleAction == null)
                    {
                        throw new ArgumentNullException(nameof(options.SimpleScheduleAction));
                    }
                    triggerBuilder = TriggerBuilder.Create().WithIdentity(triggerName, jobGroup).WithSimpleSchedule(options.SimpleScheduleAction);
                    break;
                default:
                    throw new Exception($"Unsupported ScheduleType：{options.ScheduleType}");
            }

            // 当使用 Cron 表达式时，StartNow 方法不会起任何效果，Cron 有其自己的执行时间。目前看来 StartNow 应该只适用于 SimpleTrigger 触发器。
            ITrigger jobTrigger = options.IsStartNow && options.ScheduleType != ScheduleType.CronSchedule ? triggerBuilder.StartNow().Build() : triggerBuilder.Build();

            //await _scheduler.AddJob(jobDetail, true);
            await _scheduler.ScheduleJob(jobDetail, jobTrigger);

            if (options.IsStartNow && options.ScheduleType == ScheduleType.CronSchedule)
            {
                var triggerNameStartNow = $"{triggerName}StartNow";
                ITrigger jobTriggerStartNow = TriggerBuilder.Create().WithIdentity(triggerNameStartNow, jobGroup).StartNow().ForJob(jobDetail).Build();
                await _scheduler.ScheduleJob(jobTriggerStartNow);
            }
        }
        public async Task ScheduleJob(IJobDetail jobDetail, ITrigger jobTrigger)
        {
            await _scheduler.ScheduleJob(jobDetail, jobTrigger);
        }
        public async Task ScheduleJob(ITrigger jobTrigger)
        {
            // 触发器绑定Job：TriggerBuilder.Create().xxx.ForJob(jobDetail).Build();
            // 注：每个Job可以绑定多个Trigger，但一个Trigger只能绑定一个任务。
            await _scheduler.ScheduleJob(jobTrigger);
        }

        /// <summary>
        /// 启动定时服务
        /// </summary>
        public async Task Start(TimeSpan? delay = null, CancellationToken cancellationToken = default)
        {
            if (_scheduler.IsStarted)
            {
                return;
            }

            if (delay != null)
                await _scheduler.StartDelayed(delay.Value, cancellationToken);
            else
                await _scheduler.Start(cancellationToken);
        }
        /// <summary>
        /// 关闭定时服务
        /// </summary>
        public async Task Stop(bool waitForJobsToComplete, CancellationToken cancellationToken = default)
        {
            await _scheduler.Shutdown(waitForJobsToComplete, cancellationToken);
        }
    }
}
