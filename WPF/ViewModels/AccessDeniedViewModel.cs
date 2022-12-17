using System.Windows.Input;
using WPF.Commands;
using WPF.State.Navigators;

namespace WPF.ViewModels;

public class AccessDeniedViewModel : ViewModelBase
{
    public ViewModelBase? PreviousViewModel { get; }
    
    private readonly INavigator _navigator;

    public ICommand GoBackCommand => new RelayCommand<ViewModelBase>(model =>
    {
        _navigator.CurrentViewModel = model;
    });

    public AccessDeniedViewModel(ViewModelBase? previousViewModel, INavigator navigator)
    {
        PreviousViewModel = previousViewModel;
        _navigator = navigator;
    }
}