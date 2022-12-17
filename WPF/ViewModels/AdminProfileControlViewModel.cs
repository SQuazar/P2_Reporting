using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models;
using Domain.Services;
using HandyControl.Controls;
using HandyControl.Data;
using WPF.Commands;

namespace WPF.ViewModels;

public partial class AdminProfileControlViewModel : ViewModelBase, IAccessibleViewModel
{
    [ObservableProperty] private Account? _account;

    public int AccessLevel => Convert.ToInt32(Domain.Models.AccessLevel.Admin);

    #region Services

    private readonly IRoleService _roleService;
    private readonly IAccountRoleService _accountRoleService;

    #endregion

    #region AdminControl

    [ObservableProperty] private ObservableCollection<Role> _roles = new();
    [ObservableProperty] private ObservableCollection<Role> _accountRoles = new();

    #region Admin commands region

    public ICommand SaveRoles => new AsyncRelayCommand<IEnumerable<Role>>(
        async roles =>
        {
            if (roles == null) throw new Exception();
            var enumerable = roles as Role[] ?? roles.ToArray();
            var removedRoles = _account!.Roles.Where(r => !enumerable.Contains(r)).Select(r => r.Id);
            var addedRoles = enumerable.Where(r => !_account.Roles.Contains(r)).Select(r => r.Id);

            await _accountRoleService.RemoveRoles(_account, removedRoles);
            var updated = await _accountRoleService.AddRoles(_account, addedRoles);
            _account.Roles = updated.Roles;
            AccountRoles = new ObservableCollection<Role>(_account.Roles);
            AccountRoles.CollectionChanged += AccountRolesCollectionChanged;
            InitRoles.Execute(null);
            OnPropertyChanged(nameof(Account));
            RolesUpdated?.Invoke();
            Growl.Success(new GrowlInfo
            {
                Message = "Изменения сохранены",
                WaitTime = 4,
                Token = "GrowlMsg"
            });
        }, roles => roles != null && roles.Any() && 
                    _account != null && !Equals(_account, Account.Empty));

    public ICommand AddRole => new RelayCommand<Role>(role =>
    {
        AccountRoles.Add(role!);
        Roles.Remove(role!);
    });

    public ICommand InitRoles => new AsyncRelayCommand(async _ =>
    {
        _roles.Clear();
        var roles = await _roleService.GetAll();
        foreach (var role in roles.Where(r => !_accountRoles.Contains(r)))
            _roles.Add(role);
    });

    #endregion

    #endregion

    public event Action? RolesUpdated;

    public AdminProfileControlViewModel(Account? account, IRoleService roleService,
        IAccountRoleService accountRoleService)
    {
        _account = account ?? Account.Empty;
        _roleService = roleService;
        _accountRoleService = accountRoleService;
        foreach (var role in _account.Roles)
            _accountRoles.Add(role);
        AccountRoles.CollectionChanged += AccountRolesCollectionChanged;
        InitRoles.Execute(null);
    }

    private void AccountRolesCollectionChanged(object? o, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Remove) return;
        Role? removed = null;
        if (e.NewItems == null)
        {
            removed = e.OldItems![0] as Role;
        }
        else
        {
            foreach (var oldItem in e.OldItems!)
            {
                if (!e.NewItems.Contains(oldItem))
                    removed = oldItem as Role;
            }
        }

        if (removed != null)
            Roles.Add(removed);
    }
}