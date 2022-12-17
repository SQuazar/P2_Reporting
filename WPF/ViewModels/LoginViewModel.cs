using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Commands;
using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels.Factories;

namespace WPF.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private string? _username = "user";
    [ObservableProperty] private string? _password;
    [ObservableProperty] private string? _errorText;

    public ICommand Login => new LoginAsyncCommand(this, _authenticator, _navigator, _viewModelFactory);
    public ICommand ChangeViewModel => new ChangeViewModelCommand(_navigator, _viewModelFactory, _authenticator);

    private readonly IAuthenticator _authenticator;
    private readonly INavigator _navigator;
    private readonly IViewModelFactory _viewModelFactory;

    public LoginViewModel(IAuthenticator authenticator, INavigator navigator, IViewModelFactory viewModelFactory)
    {
        _authenticator = authenticator;
        _navigator = navigator;
        _viewModelFactory = viewModelFactory;
    }
}