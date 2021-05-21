using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sean.Core.Ioc;
using Sean.Core.Quartz.Extensions;
using Sean.Core.Topshelf;
using Sean.Core.Topshelf.Extensions;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;
using Sean.Utility.Impls.Log;
using Topshelf;

namespace Example.Topshelf.JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            IocContainer.Instance.ConfigureServices(services =>
            {
                services.AddTransient(typeof(ISimpleLogger<>), typeof(SimpleLocalLogger<>));
                services.ConfigureServiceOptions();// 使用默认配置：appsettings.json
                services.AddQuartzJobs();
                services.AddSingleton<MainService>();
            });

            SimpleLocalLoggerBase.DateTimeFormat = time => time.ToLongDateTime();

            //var logger = ServiceManager.GetService<ISimpleLogger<Program>>();
            //var configuration = ServiceManager.GetService<IConfiguration>();

            var serviceManager = new HostedServiceManager(options => { });
            //serviceManager.RunService<MainService>((x, options) => { });
            serviceManager.Run((x, options) =>
            {
                x.Service<MainService>(sc =>
                {
                    sc.ConstructUsing(settings =>
                    {
                        var service = IocContainer.Instance.GetService<MainService>();
                        service.Settings = settings;
                        return service;
                    });
                    sc.WhenStarted(c => c.Start());
                    sc.WhenStopped(c => c.Stop());
                });
            });
        }
    }
}
