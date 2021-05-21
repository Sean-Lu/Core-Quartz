#if NETSTANDARD
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
                services.AddDI(type, diType);
            }

            JobManager.DefaultJobFactory = jobFactoryFunc != null ? jobFactoryFunc(services) : new IocJobFactory(services.BuildServiceProvider());
        }

        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="diType"></param>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static void AddDI(this IServiceCollection services, Type type, ServiceLifetime diType)
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
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="diType"></param>
        private static void AddDI<T>(this IServiceCollection services, ServiceLifetime diType) where T : class
        {
            switch (diType)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient<T>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<T>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<T>();
                    break;
            }
        }
    }
}
#endif