using System;
using System.Threading.Tasks;
using Quartz;
using Sean.Core.Ioc;
using Sean.Core.Quartz;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;

namespace Example.Topshelf.JobService.Jobs
{
    /// <summary>
    /// 测试Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class TestJob : JobBase
    {
        private readonly ILogger _logger;

        public TestJob()
        {
            _logger = IocContainer.Instance.GetService<ISimpleLogger<TestJob>>(); ;
        }

        protected override Task ExecuteJob(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            _logger.LogInfo($"【{jobName}】{DateTime.Now.ToLongDateTime()}");
            return Task.CompletedTask;
        }
    }
}
