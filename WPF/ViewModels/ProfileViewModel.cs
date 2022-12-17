using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models;
using Domain.Services;
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.AspNet.Identity;
using WPF.Attributes;
using WPF.Commands;
using WPF.State.Authenticators;

namespace WPF.ViewModels;

[ProtectedViewModel(AccessLevel.User)]
public partial class ProfileViewModel : ViewModelBase
{
    private readonly IAuthenticator _authenticator;

    #region Services

    private readonly IAccountService _accountService;
    private readonly IRoleService _roleService;
    private readonly IAccountRoleService _accountRoleService;
    private readonly IPasswordHasher _passwordHasher;

    #endregion

    #region Account data

    public Account Account => _account;

    private Account _account;
    [ObservableProperty] private string _firstname;
    [ObservableProperty] private string _surname;
    [ObservableProperty] private string? _middleName;
    [ObservableProperty] private string? _inputPassword;
    [ObservableProperty] private string? _confirmPassword;

    #endregion

    #region AdminControl

    public AdminProfileControlViewModel? AdminViewModel
    {
        get
        {
            var model = new AdminProfileControlViewModel(_account, _roleService, _accountRoleService);
            var current = _authenticator.CurrentAccount ?? Account.Empty;
            model = CanAccess(typeof(AdminProfileControlViewModel), current) ? model : null;
            if (model != null)
                model.RolesUpdated += () =>
                {
                    OnPropertyChanged(nameof(Account));
                    if (Account.Equals(_authenticator.CurrentAccount))
                        _authenticator.CurrentAccount = Account;
                    OnPropertyChanged();
                };
            return model;
        }
    }

    #endregion

    #region Commands

    public ICommand SaveCommand => new AsyncRelayCommand<Account>(async account =>
        {
            if (InputPassword != ConfirmPassword)
            {
                Growl.Error(new GrowlInfo
                {
                    Message = "Пароли не совпадают!",
                    WaitTime = 4,
                    Token = "GrowlMsg"
                });
                return;
            }

            if (account.PasswordHash != InputPassword)
            {
                account.PasswordHash = _passwordHasher.HashPassword(InputPassword);
                InputPassword = account.PasswordHash;
                ConfirmPassword = InputPassword;
            }

            _account.Firstname = Firstname;
            _account.Surname = Surname;
            _account.MiddleName = MiddleName;
            _account = await _accountService.Update(account.Id, account);
            if (_account.Equals(_authenticator.CurrentAccount))
                _authenticator.CurrentAccount = _account;
            Growl.Success(new GrowlInfo
            {
                Message = "Изменения сохранены",
                WaitTime = 5,
                Token = "GrowlMsg"
            });
            OnPropertyChanged(nameof(Account));
            OnPropertyChanged(nameof(AdminViewModel));
        }, account => account != null &&
                      !(
                          string.IsNullOrEmpty(InputPassword) ||
                          string.IsNullOrEmpty(Firstname) ||
                          string.IsNullOrEmpty(Surname)
                      )
    );

    #endregion

    public ProfileViewModel(Account? account, IAccountService accountService, IAuthenticator authenticator,
        IRoleService roleService,
        IAccountRoleService accountRoleService,
        IPasswordHasher passwordHasher)
    {
        _authenticator = authenticator;
        _accountService = accountService;
        _account = account ?? Account.Empty;
        InputPassword = _account.PasswordHash;
        ConfirmPassword = InputPassword;
        _firstname = _account.Firstname;
        _surname = _account.Surname;
        _middleName = _account.MiddleName;
        _passwordHasher = passwordHasher;

        _roleService = roleService;
        _accountRoleService = accountRoleService;
    }
}