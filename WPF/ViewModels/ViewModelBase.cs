using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF.ViewModels;

public class ViewModelBase : ObservableObject
{
    public delegate TViewModel CreateViewModel<out TViewModel>() where TViewModel : ViewModelBase;

    public virtual void Dispose() {}

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}