using System.Windows;
using EntityFramework;
using HandyControl.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WPF.HostBuilders;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        static IHostBuilder CreateHostBuilder(string[]? args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .AddConfiguration()
                .AddSecurity()
                .AddDbContext()
                .AddServices()
                .AddStore()
                .AddViewModels()
                .AddViews();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            var dbContextFactory = _host.Services.GetRequiredService<ReportingDbContextFactory>();
            await using var dbContext = dbContextFactory.CreateDbContext();

            ConfigHelper.Instance.SetLang("ru");
            
            var window = _host.Services.GetRequiredService<MainWindow>();
            window.Show();
            base.OnStartup(e);
        }
    }
}