#if !NETFRAMEWORK
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using Sean.Core.Quartz.Factory;

namespace Sean.Core.Quartz.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="diType"></param>
        /// <param name="assembly"></param>
        /// <param name="jobFactoryFunc">默认使用<see cref="IocJobFactory"/></param>
        public static void AddQuartzJobs(this IServiceCollection services, ServiceLifetime diType = ServiceLifetime.Transient, Assembly assembly = null, Func<IServiceCollection, IJobFactory> jobFactoryFunc = null)
        {
            //services.AddScoped<TestJob>();

            var typesImpl = (assembly ?? Assembly.GetCallingAssembly()).GetTypes().Where(c => c.IsClass && typeof(IJob).IsAssignableFrom(c));
            foreach (var type in typesImpl)
            {
                switch (diType)
                {
                    case ServiceLifetime.Transient:
                        services.AddTransient(type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(type);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(type);
                        break;
                }
            }

            JobManager.DefaultJobFactory = jobFactoryFunc != null ? jobFactoryFunc(services) : new IocJobFactory(services.BuildServiceProvider());
        }
    }
}
#endif