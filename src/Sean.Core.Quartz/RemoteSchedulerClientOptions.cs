namespace Sean.Core.Quartz
{
    /// <summary>
    /// 远程调度配置（客户端）
    /// </summary>
    public class RemoteSchedulerClientOptions
    {
        /// <summary>
        /// 服务端代理地址，示例：tcp://127.0.0.1:9192/QuartzScheduler
        /// </summary>
        public string ProxyAddress { get; set; }
    }
}