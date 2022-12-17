using System.Threading.Tasks;
using Domain.Exceptions;
using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels;
using WPF.ViewModels.Factories;

namespace WPF.Commands;

public class LoginAsyncCommand : AsyncCommandBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly IAuthenticator _authenticator;
    private readonly INavigator _navigator;
    private readonly IViewModelFactory _viewModelFactory;

    public LoginAsyncCommand(LoginViewModel loginViewModel, IAuthenticator authenticator, INavigator navigator,
        IViewModelFactory viewModelFactory)
    {
        _loginViewModel = loginViewModel;
        _authenticator = authenticator;
        _navigator = navigator;
        _viewModelFactory = viewModelFactory;
    }

    public override async Task ExecuteAsync(object? parameter)
    {
        try
        {
            await _authenticator.Login(_loginViewModel.Username!, _loginViewModel.Password!);
            _loginViewModel.ErrorText = null;
            new ChangeViewModelCommand(_navigator, _viewModelFactory, _authenticator)
                .Execute(MainViewModelFactory.Type.Home);
        }
        catch (UserNotFoundException e)
        {
            _loginViewModel.ErrorText = $"Пользователь {e.Username} не найден";
        }
        catch (InvalidPasswordException)
        {
            _loginViewModel.ErrorText = "Неверный пароль";
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter) &&
               !(string.IsNullOrEmpty(_loginViewModel.Password) || string.IsNullOrEmpty(_loginViewModel.Username));
    }
}