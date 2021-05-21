using Quartz;
using Quartz.Impl.Matchers;
using Sean.Core.Quartz;
using System;

namespace Example.RemoteScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = "127.0.0.1";
            var options = new RemoteSchedulerServerOptions();
            var proxyAddress = $"{options.ChannelType}://{server}:{options.Port}/{options.BindName}";//"tcp://127.0.0.1:9192/QuartzScheduler";
            var jobName = "TestJob";
            var jobKey = new JobKey(jobName, $"{jobName}Group");

            #region IRemotableQuartzScheduler
            //var proxy = JobManager.GetRemoteSchedulerProxy(proxyAddress);
            //if (proxy.CheckExists(jobKey))
            //{
            //    while (Console.ReadLine() == "1")
            //    {
            //        proxy.TriggerJob(jobKey, null);
            //        Console.WriteLine($"Job执行成功：{jobKey.Name}");
            //    }
            //}
            #endregion

            #region IScheduler
            var scheduler = JobManager.GetRemoteScheduler(proxyAddress).Result;

            //var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result;
            //foreach (var key in jobKeys)
            //{
            //    Console.WriteLine($"JobKey: {key}");
            //}

            if (scheduler.CheckExists(jobKey).Result)
            {
                while (Console.ReadLine() == "1")
                {
                    scheduler.TriggerJob(jobKey).ContinueWith(task =>
                    {
                        if (!task.IsFaulted && !task.IsCanceled)
                        {
                            Console.WriteLine($"Job执行成功：{jobKey.Name}");
                        }
                    }).Wait();
                }
            }
            else
            {
                Console.WriteLine($"Job不存在：{jobKey.Name}");
            }
            #endregion

            Console.ReadLine();
        }
    }
}
