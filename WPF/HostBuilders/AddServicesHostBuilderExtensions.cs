using Domain.Services;
using Domain.Services.AuthenticationServices;
using EntityFramework.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WPF.HostBuilders;

public static class AddServicesHostBuilderExtensions
{
    public static IHostBuilder AddServices(this IHostBuilder host)
    {
        host.ConfigureServices((ctx, services) =>
        {
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IAccountRoleService, AccountRoleService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IReportService, ReportService>();
        });
        return host;
    }
}