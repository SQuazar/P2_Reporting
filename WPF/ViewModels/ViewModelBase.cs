using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models;
using WPF.Attributes;

namespace WPF.ViewModels;

public class ViewModelBase : ObservableObject
{
    public delegate TViewModel CreateViewModel<out TViewModel>() where TViewModel : ViewModelBase;

    public virtual void Dispose()
    {
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public static bool CanAccess(MemberInfo viewModel, Account? account)
    {
        ProtectedViewModelAttribute? attribute;
        if ((attribute =
                Attribute.GetCustomAttribute(viewModel, typeof(ProtectedViewModelAttribute)) as
                    ProtectedViewModelAttribute) == null) return true;
        if (account == null) return false;
        return account.AccessLevel >= Convert.ToInt32(attribute.AccessLevel);
    }

    public static int GetAccessLevel(MemberInfo viewModel)
    {
        ProtectedViewModelAttribute? attribute;
        return (attribute =
            Attribute.GetCustomAttribute(viewModel,
                typeof(ProtectedViewModelAttribute)) as ProtectedViewModelAttribute) == null
            ? 0
            : Convert.ToInt32(attribute.AccessLevel);
    }
}