using Quartz;
using System;
using System.Threading.Tasks;

namespace Example.Simple.ConsoleApp.Job
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            Console.WriteLine($"【{jobName}】{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            return Task.CompletedTask;
        }
    }
}
