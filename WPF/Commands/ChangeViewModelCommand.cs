using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels;
using WPF.ViewModels.Factories;

namespace WPF.Commands;

public class ChangeViewModelCommand : CommandBase
{
    private readonly INavigator _navigator;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IAuthenticator _authenticator;

    public ChangeViewModelCommand(INavigator navigator, IViewModelFactory viewModelFactory, IAuthenticator authenticator)
    {
        _navigator = navigator;
        _viewModelFactory = viewModelFactory;
        _authenticator = authenticator;
    }

    public override void Execute(object? parameter)
    {
        if (parameter?.GetType() != _viewModelFactory.GetRequiredType()) return;
        var model = _viewModelFactory.Create(parameter);
        if (model is IAccessibleViewModel accessibleViewModel)
        {
            if (_authenticator.CurrentAccount == null)
            {
                _navigator.CurrentViewModel = new AccessDeniedViewModel(_navigator.CurrentViewModel, _navigator);
                return;
            }

            if (_authenticator.CurrentAccount.AccessLevel < accessibleViewModel.AccessLevel)
            {
                _navigator.CurrentViewModel = new AccessDeniedViewModel(_navigator.CurrentViewModel, _navigator);
                return;
            }
            if (model is LoginViewModel or RegistrationViewModel)
                _authenticator.Logout();

        }
        
        _navigator.CurrentViewModel = model;
    }
}