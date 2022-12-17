using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WPF.HostBuilders;

public static class AddSecurityHostBuilderExtensions
{
    public static IHostBuilder AddSecurity(this IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
        });
        return builder;
    }
}