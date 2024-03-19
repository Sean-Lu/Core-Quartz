using System;
using System.Collections.Generic;
using Example.Simple.ConsoleApp.Job;
using Sean.Core.Quartz;

namespace Example.Simple.ConsoleApp
{
    internal class Program
    {
        private static readonly JobManager _jobManager = new JobManager();
        static void Main(string[] args)
        {
            var list = new List<JobOptions>
            {
                //new JobOptions
                //{
                //    JobType = typeof(TestJob),
                //    ScheduleType = ScheduleType.SimpleSchedule,
                //    SimpleScheduleAction = c => c.WithIntervalInSeconds(1).RepeatForever(), // 每秒执行1次
                //    IsStartNow = false
                //},
                new JobOptions
                {
                    JobType = typeof(TestJob),
                    ScheduleType = ScheduleType.CronSchedule,
                    CronExpression = "*/1 * * * * ?", // 每秒执行1次
                    IsStartNow = false
                }
            };
            list.ForEach(c => _jobManager.ScheduleJob(c).Wait());
            _jobManager.Start().Wait();

            Console.ReadLine();

            _jobManager.Stop(true).Wait();
        }
    }
}
