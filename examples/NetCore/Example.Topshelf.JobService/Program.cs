using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sean.Core.Quartz.Extensions;
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
            IocContainer.ConfigureServices((services, configuration) =>
            {
                services.AddTransient(typeof(ISimpleLogger<>), typeof(SimpleLocalLogger<>));
                services.AddQuartzJobs();
                services.AddSingleton<MainService>();
            });

            SimpleLocalLoggerBase.DateTimeFormat = time => time.ToLongDateTime();

            HostFactory.Run(x =>
            {
                IConfiguration configuration = IocContainer.GetService<IConfiguration>();
                var serviceName = configuration.GetValue<string>("Service:Name");
                var serviceDisplayName = configuration.GetValue<string>("Service:DisplayName");
                var serviceDescription = configuration.GetValue<string>("Service:Description");

                x.SetServiceName(serviceName);
                x.SetDisplayName(serviceDisplayName);
                x.SetDescription(serviceDescription);

                x.Service<MainService>(sc =>
                {
                    sc.ConstructUsing(settings =>
                    {
                        var service = IocContainer.GetService<MainService>();
                        service.Settings = settings;
                        return service;
                    });
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                    //sc.WhenPaused(s => s.Pause());
                    //sc.WhenContinued(s => s.Continue());
                    //sc.WhenShutdown(s => s.Shutdown());
                });

                // 服务启动类型
                x.StartAutomatically();

                // 服务运行身份
                x.RunAsLocalSystem();

                //x.EnablePauseAndContinue();
                //x.EnableShutdown();

                // 服务依赖项
                //x.DependsOn(name);

                //x.BeforeInstall(settings => { logger.LogInfo("Install service => Start"); });
                //x.AfterInstall(settings => { logger.LogInfo("Install service => End"); });
                //x.BeforeUninstall(() => { logger.LogInfo("Uninstall service => Start"); });
                //x.AfterUninstall(() => { logger.LogInfo("Uninstall service => End"); });

                // 自动恢复设置（服务重启）
                x.EnableServiceRecovery(r =>
                {
                    var delay = 0;
                    // 第1次失败：重启服务
                    r.RestartService(delay);

                    // 第2次失败：运行指定外部程序
                    //r.RunProgram(delay, "command");

                    // 第3次失败：重启计算机
                    //r.RestartComputer(delay, string.Format("Service {0} crashed!", serviceName));

                    // 仅服务崩溃时重启服务
                    r.OnCrashOnly();

                    // 恢复计算周期
                    r.SetResetPeriod(1);
                });
            });
        }
    }
}
