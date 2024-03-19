using System;
using System.Threading.Tasks;
using Quartz;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;

namespace Example.Topshelf.JobService.Jobs
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private readonly ILogger _logger;

        public TestJob()
        {
            _logger = DIManager.Resolve<ISimpleLogger<TestJob>>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            _logger.LogInfo($"【{jobName}】{DateTime.Now.ToLongDateTime()}");
            return Task.CompletedTask;
        }
    }
}
