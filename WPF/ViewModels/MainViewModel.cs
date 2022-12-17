using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WPF.Commands;
using WPF.Controls;
using WPF.State.Authenticators;
using WPF.State.Navigators;
using WPF.ViewModels.Factories;

namespace WPF.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IAuthenticator _authenticator;
    private readonly INavigator _navigator;
    private readonly IViewModelFactory _viewModelFactory;

    public ViewModelBase? CurrentViewModel => _navigator.CurrentViewModel;
    public bool IsLoggedIn => _authenticator.IsLoggedIn;
    public MainNavigationBar MainNavigationBar { get; private set; }

    #region Navigation buttons visibility

    public Visibility HomeButtonVisibility =>
        CanAccess(_viewModelFactory.Create(MainViewModelFactory.Type.Home) as IAccessibleViewModel)
            ? Visibility.Visible
            : Visibility.Collapsed;
    
    public Visibility ReportsButtonVisibility =>
        CanAccess(_viewModelFactory.Create(MainViewModelFactory.Type.Reports) as IAccessibleViewModel)
            ? Visibility.Visible
            : Visibility.Collapsed;
    
    public Visibility ProfileButtonVisibility =>
        CanAccess(_viewModelFactory.Create(MainViewModelFactory.Type.Profile) as IAccessibleViewModel)
            ? Visibility.Visible
            : Visibility.Collapsed;
    
    public Visibility AccountsButtonVisibility =>
        CanAccess(_viewModelFactory.Create(MainViewModelFactory.Type.Accounts) as IAccessibleViewModel)
            ? Visibility.Visible
            : Visibility.Collapsed;
    
    public Visibility ReportDocumentsButtonVisibility =>
        CanAccess(_viewModelFactory.Create(MainViewModelFactory.Type.Documentation) as IAccessibleViewModel)
            ? Visibility.Visible
            : Visibility.Collapsed;

    #endregion

    # region Commands

    public ICommand ChangeViewModelCommand { get; }
    public ICommand Logout => new RelayCommand(_ => _authenticator.Logout());
    
    public ICommand OpenUrl => new AsyncRelayCommand<string>(uri =>
    {
        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
    });

    #endregion

    public MainViewModel(IAuthenticator authenticator, INavigator.ResolveNavigator resolveNavigator,
        MainViewModelFactory viewModelFactory)
    {
        _authenticator = authenticator;
        _navigator = resolveNavigator(INavigator.Type.Main);
        _viewModelFactory = viewModelFactory;
        _authenticator.StateChanged += AuthenticatorStateChanged;
        _navigator.StateChanged += NavigatorStateChanged;
        ChangeViewModelCommand = new ChangeViewModelCommand(_navigator, _viewModelFactory, authenticator);
        ChangeViewModelCommand.Execute(MainViewModelFactory.Type.Login);
        MainNavigationBar = new MainNavigationBar();
    }

    private void NavigatorStateChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    private void AuthenticatorStateChanged()
    {
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(HomeButtonVisibility));
        OnPropertyChanged(nameof(ReportsButtonVisibility));
        OnPropertyChanged(nameof(ProfileButtonVisibility));
        OnPropertyChanged(nameof(AccountsButtonVisibility));
        OnPropertyChanged(nameof(ReportDocumentsButtonVisibility));

        if (!IsLoggedIn)
        {
            ChangeViewModelCommand.Execute(MainViewModelFactory.Type.Login);
            MainNavigationBar = new MainNavigationBar();
        }
        OnPropertyChanged(nameof(MainNavigationBar));
    }

    private bool CanAccess(IAccessibleViewModel? accessibleViewModel)
    {
        if (accessibleViewModel == null) return false;
        return IsLoggedIn && _authenticator.CurrentAccount?.AccessLevel >= accessibleViewModel.AccessLevel;
    }

    public override void Dispose()
    {
        _authenticator.StateChanged -= AuthenticatorStateChanged;
        _navigator.StateChanged -= NavigatorStateChanged;
    }
}