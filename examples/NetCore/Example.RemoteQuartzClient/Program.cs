using Quartz;
using Sean.Core.Quartz;
using System;

namespace Example.RemoteScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // 远程调度目前只支持.NET Framework，不支持.NET Core，因为底层是基于Remoting实现的（在.NET Core中废弃了Remoting，微软并没有在.NET Core中支持Remoting的计划。）

            //var proxyAddress = "tcp://127.0.0.1:9192/QuartzScheduler";
            //var scheduler = JobManager.GetRemoteScheduler(proxyAddress, true).Result;

            //var jobKey = new JobKey("TestJob", "TestJobGroup");
            //if (scheduler.CheckExists(jobKey).Result)
            //{
            //    while (Console.ReadLine() == "1")
            //    {
            //        scheduler.TriggerJob(jobKey).ContinueWith(task =>
            //        {
            //            if (!task.IsFaulted && !task.IsCanceled)
            //            {
            //                Console.WriteLine($"Job执行成功：{jobKey.Name}");
            //            }
            //        }).Wait();
            //    }
            //}
            //else
            //{
            //    Console.WriteLine($"Job不存在：{jobKey.Name}");
            //}

            Console.ReadLine();
        }
    }
}
