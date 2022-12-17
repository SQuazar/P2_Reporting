using Domain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels;
using WPF.ViewModels.Factories;

namespace WPF.HostBuilders;

public static class AddViewModelsHostBuilderExtensions
{
    public static IHostBuilder AddViewModels(this IHostBuilder host)
    {
        host.ConfigureServices((ctx, services) =>
        {
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<AccountsViewModel>();
            services.AddTransient<ReportingDocumentationViewModel>();
            services.AddTransient<MainViewModel>();

            services.AddSingleton<ViewModelBase.CreateViewModel<HomeViewModel>>(provider =>
                provider.GetRequiredService<HomeViewModel>);
            services.AddSingleton<ViewModelBase.CreateViewModel<ReportsViewModel>>(provider =>
                provider.GetRequiredService<ReportsViewModel>);
            services.AddSingleton<ViewModelBase.CreateViewModel<ProfileViewModel>>(provider => () =>
                new ProfileViewModel
                (
                    provider.GetRequiredService<IAuthenticator>().CurrentAccount,
                    provider.GetRequiredService<IAccountService>(),
                    provider.GetRequiredService<IAuthenticator>(),
                    provider.GetRequiredService<IRoleService>(),
                    provider.GetRequiredService<IAccountRoleService>(),
                    provider.GetRequiredService<IPasswordHasher>()
                ));
            services.AddSingleton<ViewModelBase.CreateViewModel<AccountsViewModel>>(provider =>
                provider.GetRequiredService<AccountsViewModel>);
            services.AddSingleton<ViewModelBase.CreateViewModel<ReportingDocumentationViewModel>>(provider =>
                provider.GetRequiredService<ReportingDocumentationViewModel>);
            services.AddSingleton<ViewModelBase.CreateViewModel<LoginViewModel>>(provider => () => new LoginViewModel
            (
                provider.GetRequiredService<IAuthenticator>(),
                provider.GetRequiredService<INavigator.ResolveNavigator>()(INavigator.Type.Main),
                provider.GetRequiredService<MainViewModelFactory>()
            ));
            services.AddSingleton<ViewModelBase.CreateViewModel<RegistrationViewModel>>(provider => () =>
                new RegistrationViewModel
                (
                    provider.GetRequiredService<IAuthenticator>(),
                    provider.GetRequiredService<INavigator.ResolveNavigator>()(INavigator.Type.Main),
                    provider.GetRequiredService<MainViewModelFactory>()
                ));

            services.AddSingleton<MainViewModelFactory>();
        });
        return host;
    }
}