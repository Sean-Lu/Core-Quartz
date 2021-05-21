using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Example.Hosting.JobService.Jobs;
using Microsoft.Extensions.Hosting;
using Sean.Core.Quartz;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;
using Sean.Utility.Impls.Log;

namespace Example.Hosting.JobService
{
    public class MainService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly JobManager _jobManager;

        public MainService(ISimpleLogger<MainService> logger)
        {
            _logger = logger;
            _jobManager = new JobManager();

            SimpleLocalLoggerBase.DateTimeFormat = time => time.ToLongDateTime();

            var list = new List<JobOptions>
            {
                new JobOptions
                {
                    JobType = typeof(TestJob),
                    ScheduleType = ScheduleType.SimpleSchedule,
                    SimpleScheduleAction = c => c.WithIntervalInSeconds(1).RepeatForever(), // 每秒执行1次
                    IsStartNow = true
                },
                //new JobOptions
                //{
                //    JobType = typeof(TestJob),
                //    ScheduleType = ScheduleType.CronSchedule,
                //    CronExpression = "*/1 * * * * ?", // 每秒执行1次
                //    IsStartNow = true
                //}
            };

            list.ForEach(c => _jobManager.ScheduleJob(c).Wait());
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _jobManager.Start(null, cancellationToken);
            _logger.LogInfo($"开始运行Job服务：{typeof(MainService).Namespace}");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _jobManager.Stop(true, cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
