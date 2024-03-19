namespace Sean.Core.Quartz
{
    /// <summary>
    /// 远程调度配置（服务端）
    /// </summary>
    public class RemoteSchedulerServerOptions
    {
        /// <summary>
        /// 是否支持远程调度
        /// </summary>
        public bool EnableRemoteScheduler { get; set; }

        /// <summary>
        /// 端口，默认值：9192
        /// </summary>
        public int Port { get; set; } = 9192;

#if NETFRAMEWORK
        /// <summary>
        /// 协议类型，默认值：tcp
        /// </summary>
        public string ChannelType { get; set; } = "tcp";// RemotingSchedulerExporter.ChannelTypeTcp

        /// <summary>
        /// 绑定名称，默认值：QuartzScheduler
        /// </summary>
        public string BindName { get; set; } = "QuartzScheduler";
#endif
    }
}
