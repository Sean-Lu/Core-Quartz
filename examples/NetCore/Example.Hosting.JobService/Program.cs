using Microsoft.Extensions.Hosting;
using Sean.Core.Hosting;
using Sean.Core.Quartz.Extensions;
using Sean.Utility.Extensions;

namespace Example.Hosting.JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostBuilderHelper.CreateDefaultBuilder<MainService>(args, services =>
            {
                services.AddSimpleLocalLogger();
                services.AddQuartzJobs();
            }).UseConsoleLifetime().Build().Run();
        }
    }
}
