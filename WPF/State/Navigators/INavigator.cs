using System;
using WPF.ViewModels;

namespace WPF.State.Navigators;

public interface INavigator
{
    public delegate INavigator ResolveNavigator(Type type);

    public ViewModelBase? CurrentViewModel { get; set; }

    public event Action? StateChanged;

    public enum Type
    {
        Main
    }
}