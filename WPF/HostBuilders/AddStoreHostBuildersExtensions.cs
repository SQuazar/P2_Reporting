using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WPF.State.Accounts;
using WPF.State.Authenticators;
using WPF.State.Navigators;

namespace WPF.HostBuilders;

public static class AddStoreHostBuildersExtensions
{
    public static IHostBuilder AddStore(this IHostBuilder host)
    {
        host.ConfigureServices((ctx, services) =>
        {

            services.AddSingleton<MainNavigator>();
            services.AddTransient<INavigator.ResolveNavigator>(provider => key =>
            {
                return key switch
                {
                    INavigator.Type.Main => provider.GetRequiredService<MainNavigator>(),
                    _ => throw new ArgumentException("Cannot find navigator with selected type", nameof(key))
                };
            });

            services.AddSingleton<IAccountStore, AccountStore>();
            services.AddSingleton<IAuthenticator, Authenticator>();
        });
        return host;
    }
}