using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sean.Core.Ioc;
using Sean.Core.Topshelf;
using Sean.Utility.Contracts;
using Sean.Utility.Extensions;
using Sean.Utility.Impls.Log;

namespace Example.Topshelf.JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            IocContainer.Instance.ConfigureServices(services =>
            {
                services.AddTransient(typeof(ISimpleLogger<>), typeof(SimpleLocalLogger<>));
            });

            SimpleLocalLoggerBase.DateTimeFormat = time => time.ToLongDateTime();

            //var logger = ServiceManager.GetService<ISimpleLogger<Program>>();
            //var configuration = ServiceManager.GetService<IConfiguration>();

            var serviceManager = new HostedServiceManager(options => { });
            serviceManager.RunService<MainService>((x, options) => { });
        }
    }
}
