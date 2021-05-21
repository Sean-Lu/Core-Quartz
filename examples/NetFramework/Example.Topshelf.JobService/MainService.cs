using System.Collections.Generic;
using Example.Topshelf.JobService.Jobs;
using Sean.Core.Ioc;
using Sean.Core.Quartz;
using Sean.Core.Topshelf.Contracts;
using Sean.Utility.Contracts;
using Topshelf.Runtime;

namespace Example.Topshelf.JobService
{
    public class MainService : IHostedService
    {
        public HostSettings Settings { get; set; }

        private readonly ILogger _logger;
        private readonly JobManager _jobManager;

        public MainService()
        {
            _logger = IocContainer.Instance.GetService<ISimpleLogger<MainService>>();
            _jobManager = new JobManager(options =>
            {
                options.EnableRemoteScheduler = true;
                options.Port = 9192;
            });

            var list = new List<JobOptions>
            {
                //new JobOptions
                //{
                //    JobType = typeof(TestJob),
                //    ScheduleType = ScheduleType.SimpleSchedule,
                //    SimpleScheduleAction = c => c.WithIntervalInSeconds(1).RepeatForever(), // 每秒执行1次
                //    IsStartNow = true
                //},
                new JobOptions
                {
                    JobType = typeof(TestJob),
                    ScheduleType = ScheduleType.CronSchedule,
                    CronExpression = "0 0 12 * * ?", // 每天12:00执行1次
                    IsStartNow = true
                }
            };

            list.ForEach(c => _jobManager.ScheduleJob(c).Wait());
        }

        public void Start()
        {
            _jobManager.Start().Wait();
            _logger.LogInfo($"{Settings.ServiceName}服务启动");
        }

        public void Stop()
        {
            _jobManager.Stop(true).Wait();
            _logger.LogInfo($"{Settings.ServiceName}服务停止");
        }
    }
}
