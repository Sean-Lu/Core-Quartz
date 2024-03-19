using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sean.Core.Quartz.Extensions;
using Sean.Utility.Contracts;
using Sean.Utility.Impls.Log;

namespace Example.Hosting.JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                }).ConfigureServices(services =>
                {
                    services.AddHostedService<MainService>();
                    services.AddTransient(typeof(ISimpleLogger<>), typeof(SimpleLocalLogger<>));
                    services.AddQuartzJobs();
                }).UseConsoleLifetime().Build().Run();
        }
    }
}
