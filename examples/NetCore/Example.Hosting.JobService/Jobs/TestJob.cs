using System;
using System.Threading.Tasks;
using Quartz;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;

namespace Example.Hosting.JobService.Jobs
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private readonly ILogger _logger;

        public TestJob(ISimpleLogger<TestJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            _logger.LogInfo($"【{jobName}】{DateTime.Now.ToLongDateTime()}");
            return Task.CompletedTask;
        }
    }
}
