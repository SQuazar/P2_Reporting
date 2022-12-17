using System;
using System.Threading.Tasks;
using Domain.Services.AuthenticationServices;
using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels;
using WPF.ViewModels.Factories;

namespace WPF.Commands;

public class RegistrationAsyncCommand : AsyncCommandBase
{
    private readonly RegistrationViewModel _registrationViewModel;
    private readonly IAuthenticator _authenticator;
    private readonly INavigator _navigator;
    private readonly IViewModelFactory _viewModelFactory;

    public RegistrationAsyncCommand(RegistrationViewModel registrationViewModel, IAuthenticator authenticator,
        INavigator navigator, IViewModelFactory viewModelFactory)
    {
        _registrationViewModel = registrationViewModel;
        _authenticator = authenticator;
        _navigator = navigator;
        _viewModelFactory = viewModelFactory;
    }

    public override async Task ExecuteAsync(object? parameter)
    {
        var result = await _authenticator.Register
        (
            _registrationViewModel.Username!,
            _registrationViewModel.FirstName!,
            _registrationViewModel.Surname!,
            _registrationViewModel.MiddleName,
            _registrationViewModel.Password!,
            _registrationViewModel.ConfirmPassword!
        );
        switch (result)
        {
            case IAuthenticationService.RegistrationResult.UserAlreadyExists:
                _registrationViewModel.ErrorText = "Пользователь с таким именем уже существует";
                break;
            case IAuthenticationService.RegistrationResult.PasswordDoNotMatch:
                _registrationViewModel.ErrorText = "Пароли не совпадают";
                break;
            case IAuthenticationService.RegistrationResult.Success:
                _registrationViewModel.ErrorText = null;
                new ChangeViewModelCommand(_navigator, _viewModelFactory, _authenticator)
                    .Execute(MainViewModelFactory.Type.Login);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), "Registration result isn't handled");
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter) &&
               !(
                   string.IsNullOrEmpty(_registrationViewModel.Username) ||
                   string.IsNullOrEmpty(_registrationViewModel.FirstName) ||
                   string.IsNullOrEmpty(_registrationViewModel.Surname) ||
                   string.IsNullOrEmpty(_registrationViewModel.Password) ||
                   string.IsNullOrEmpty(_registrationViewModel.ConfirmPassword)
               );
    }
}