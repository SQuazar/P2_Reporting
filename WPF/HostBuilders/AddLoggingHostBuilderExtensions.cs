using System;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace WPF.HostBuilders;

public static class AddLoggingHostBuilderExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder host)
    {
        host.UseSerilog((ctx, provider, configuration) =>
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            configuration.WriteTo
                .File(@"logs\ErrorLog.log", rollingInterval: RollingInterval.Minute,
                    restrictedToMinimumLevel: LogEventLevel.Error);
            configuration.WriteTo
                .File($@"logs\DebugLog{timestamp}.log")
                .MinimumLevel.Debug()
                .MinimumLevel.Override(nameof(EntityFramework.Services), LogEventLevel.Debug);
            configuration.WriteTo
                .Console()
                .MinimumLevel.Debug();
        });
        return host;
    }
}