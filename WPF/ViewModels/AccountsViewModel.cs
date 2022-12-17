using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNet.Identity;
using WPF.Attributes;
using WPF.Commands;
using WPF.State.Authenticators;

namespace WPF.ViewModels;

[ProtectedViewModel(AccessLevel.Admin)]
public partial class AccountsViewModel : ViewModelBase
{
    #region Services

    private readonly IAccountService _accountService;
    private readonly IAuthenticator _authenticator;
    private readonly IRoleService _roleService;
    private readonly IAccountRoleService _accountRoleService;
    private readonly IPasswordHasher _passwordHasher;

    #endregion

    #region Properties

    [ObservableProperty] private ObservableCollection<Account> _accounts = new();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ProfileViewModel))]
    private Account? _selectedAccount;

    public ProfileViewModel ProfileViewModel => new(_selectedAccount, _accountService,
        _authenticator, _roleService,
        _accountRoleService, _passwordHasher);

    #endregion

    #region Commands

    private ICommand LoadAccounts => new AsyncRelayCommand(async _ =>
    {
        var accounts = await _accountService.GetAll();
        Accounts = new ObservableCollection<Account>(accounts);
    });

    #endregion

    public AccountsViewModel(IAccountService accountService, IAuthenticator authenticator,
        IRoleService roleService,
        IAccountRoleService accountRoleService,
        IPasswordHasher passwordHasher)
    {
        _accountService = accountService;
        _authenticator = authenticator;
        _roleService = roleService;
        _accountRoleService = accountRoleService;
        _passwordHasher = passwordHasher;
        if (authenticator.CurrentAccount != null)
            LoadAccounts.Execute(null);
    }
}