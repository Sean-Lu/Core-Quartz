using Sean.Utility.Contracts;
using Sean.Utility.Extensions;
using Sean.Utility.Impls.Log;
using System.Configuration;
using Sean.Core.DependencyInjection;
using Topshelf;

namespace Example.Topshelf.JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            DIManager.Register(container =>
            {
                container.RegisterType(typeof(ISimpleLogger<>), typeof(SimpleLocalLogger<>), ServiceLifeStyle.Transient);
            });

            SimpleLocalLoggerBase.DateTimeFormat = time => time.ToLongDateTime();

            HostFactory.Run(x =>
            {
                var serviceName = ConfigurationManager.AppSettings["ServiceName"];
                var serviceDisplayName = ConfigurationManager.AppSettings["ServiceDisplayName"];
                var serviceDescription = ConfigurationManager.AppSettings["ServiceDescription"];

                x.SetServiceName(serviceName);
                x.SetDisplayName(serviceDisplayName);
                x.SetDescription(serviceDescription);

                x.Service<MainService>(sc =>
                {
                    sc.ConstructUsing(settings =>
                    {
                        var service = DIManager.Resolve<MainService>();
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
