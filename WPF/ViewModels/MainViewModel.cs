using System.Diagnostics;
using System.Reflection;
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

    public ViewModelBase? CurrentViewModel => _navigator.CurrentViewModel;
    public bool IsLoggedIn => _authenticator.IsLoggedIn;
    public MainNavigationBar MainNavigationBar { get; private set; }

    #region Navigation buttons visibility

    public Visibility HomeButtonVisibility =>
        CanAccess(typeof(HomeViewModel)) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ReportsButtonVisibility =>
        CanAccess(typeof(ReportsViewModel)) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ProfileButtonVisibility =>
        CanAccess(typeof(ProfileViewModel)) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility AccountsButtonVisibility =>
        CanAccess(typeof(AccountsViewModel)) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ReportDocumentsButtonVisibility =>
        CanAccess(typeof(ReportingDocumentationViewModel)) ? Visibility.Visible : Visibility.Collapsed;

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
        _authenticator.StateChanged += AuthenticatorStateChanged;
        _navigator.StateChanged += NavigatorStateChanged;
        ChangeViewModelCommand = new ChangeViewModelCommand(_navigator, viewModelFactory, authenticator);
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

    private bool CanAccess(MemberInfo viewModel)
    {
        return IsLoggedIn && ViewModelBase.CanAccess(viewModel, _authenticator.CurrentAccount!);
    }

    public override void Dispose()
    {
        _authenticator.StateChanged -= AuthenticatorStateChanged;
        _navigator.StateChanged -= NavigatorStateChanged;
    }
}