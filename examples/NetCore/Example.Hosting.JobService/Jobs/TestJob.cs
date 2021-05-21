using System;
using System.Threading.Tasks;
using Quartz;
using Sean.Core.Quartz;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;

namespace Example.Hosting.JobService.Jobs
{
    /// <summary>
    /// 测试Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class TestJob : JobBase
    {
        private readonly ILogger _logger;

        public TestJob(ISimpleLogger<TestJob> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteJob(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            _logger.LogInfo($"【{jobName}】{DateTime.Now.ToLongDateTime()}");
            return Task.CompletedTask;
        }
    }
}
