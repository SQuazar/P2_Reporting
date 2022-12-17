using System;
using WPF.Attributes;
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

    public ChangeViewModelCommand(INavigator navigator, IViewModelFactory viewModelFactory,
        IAuthenticator authenticator)
    {
        _navigator = navigator;
        _viewModelFactory = viewModelFactory;
        _authenticator = authenticator;
    }

    public override void Execute(object? parameter)
    {
        if (parameter?.GetType() != _viewModelFactory.GetRequiredType()) return;
        var model = _viewModelFactory.Create(parameter);
        if (!ViewModelBase.CanAccess(model.GetType(), _authenticator.CurrentAccount))
            model = new AccessDeniedViewModel(_navigator.CurrentViewModel, _navigator);
        _navigator.CurrentViewModel = model;
    }
}