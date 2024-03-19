using Quartz;
using Sean.Core.Quartz;
using System;
using System.Threading.Tasks;
using QuartzRemoteScheduler.Client;
using QuartzRemoteScheduler.Common.Configuration;

namespace Example.RemoteScheduler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var conf = new RemoteSchedulerServerConfiguration("127.0.0.1", 9192, false);
            var schedulerFactory = new RemoteSchedulerFactory(conf);
            var scheduler = await schedulerFactory.GetScheduler();

            var jobKey = new JobKey("TestJob", "TestJobGroup");
            if (await scheduler.CheckExists(jobKey))
            {
                while (Console.ReadLine() == "1")
                {
                    await scheduler.TriggerJob(jobKey).ContinueWith(task =>
                    {
                        if (!task.IsFaulted && !task.IsCanceled)
                        {
                            Console.WriteLine($"Job执行成功：{jobKey.Name}");
                        }
                    });
                }
            }
            else
            {
                Console.WriteLine($"Job不存在：{jobKey.Name}");
            }

            Console.ReadLine();
        }
    }
}
