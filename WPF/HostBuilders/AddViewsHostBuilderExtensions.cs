using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WPF.ViewModels;

namespace WPF.HostBuilders;

public static class AddViewsHostBuilderExtensions
{
    public static IHostBuilder AddViews(this IHostBuilder host)
    {
        host.ConfigureServices((ctx, services) =>
        {
            services.AddSingleton<MainWindow>(s => new MainWindow(s.GetRequiredService<MainViewModel>()));
        });
        return host;
    }
}