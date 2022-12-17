using System;
using WPF.ViewModels;

namespace WPF.State.Navigators;

public class MainNavigator : INavigator
{
    private ViewModelBase? _currentViewModel;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            StateChanged?.Invoke();
        }
    }

    public event Action? StateChanged;
}