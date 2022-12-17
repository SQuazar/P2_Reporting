using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Commands;
using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels.Factories;

namespace WPF.ViewModels;

public partial class RegistrationViewModel : ViewModelBase
{
    [ObservableProperty] private string? _username;
    [ObservableProperty] private string? _firstName;
    [ObservableProperty] private string? _surname;
    [ObservableProperty] private string? _middleName;
    [ObservableProperty] private string? _password;
    [ObservableProperty] private string? _confirmPassword;
    [ObservableProperty] private string? _errorText;

    public ICommand Register => new RegistrationAsyncCommand(this, _authenticator, _navigator, _viewModelFactory);
    public ICommand ChangeViewModel => new ChangeViewModelCommand(_navigator, _viewModelFactory, _authenticator);

    private readonly IAuthenticator _authenticator;
    private readonly INavigator _navigator;
    private readonly IViewModelFactory _viewModelFactory;

    public RegistrationViewModel(IAuthenticator authenticator, INavigator navigator, IViewModelFactory viewModelFactory)
    {
        _authenticator = authenticator;
        _navigator = navigator;
        _viewModelFactory = viewModelFactory;
    }
}