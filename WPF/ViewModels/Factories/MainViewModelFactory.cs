using System;

namespace WPF.ViewModels.Factories;

public class MainViewModelFactory : IViewModelFactory
{
    private readonly ViewModelBase.CreateViewModel<HomeViewModel> _createHomeViewModel;
    private readonly ViewModelBase.CreateViewModel<ReportsViewModel> _createReportsViewModel;
    private readonly ViewModelBase.CreateViewModel<ProfileViewModel> _createProfileViewModel;
    private readonly ViewModelBase.CreateViewModel<AccountsViewModel> _createAccountsViewModel;

    private readonly ViewModelBase.CreateViewModel<ReportingDocumentationViewModel>
        _createReportingDocumentationViewModel;
    private readonly ViewModelBase.CreateViewModel<LoginViewModel> _createLoginViewModel;
    private readonly ViewModelBase.CreateViewModel<RegistrationViewModel> _createRegistrationViewModel;

    public MainViewModelFactory(ViewModelBase.CreateViewModel<HomeViewModel> createHomeViewModel, ViewModelBase.CreateViewModel<ReportsViewModel> createReportsViewModel, ViewModelBase.CreateViewModel<ProfileViewModel> createProfileViewModel, ViewModelBase.CreateViewModel<AccountsViewModel> createAccountsViewModel, ViewModelBase.CreateViewModel<ReportingDocumentationViewModel> createReportingDocumentationViewModel, ViewModelBase.CreateViewModel<LoginViewModel> createLoginViewModel, ViewModelBase.CreateViewModel<RegistrationViewModel> createRegistrationViewModel)
    {
        _createHomeViewModel = createHomeViewModel;
        _createReportsViewModel = createReportsViewModel;
        _createProfileViewModel = createProfileViewModel;
        _createAccountsViewModel = createAccountsViewModel;
        _createReportingDocumentationViewModel = createReportingDocumentationViewModel;
        _createLoginViewModel = createLoginViewModel;
        _createRegistrationViewModel = createRegistrationViewModel;
    }

    public ViewModelBase Create(object modelType)
    {
        if (modelType is not Type)
            throw new ArgumentException("This factory isn't supported this model type", nameof(modelType));
        return modelType switch
        {
            Type.Home => _createHomeViewModel(),
            Type.Reports => _createReportsViewModel(),
            Type.Profile => _createProfileViewModel(),
            Type.Accounts => _createAccountsViewModel(),
            Type.Documentation => _createReportingDocumentationViewModel(),
            Type.Login => _createLoginViewModel(),
            Type.Registration => _createRegistrationViewModel(),
            _ => throw new ArgumentException("Cannot find view model instance by this type", nameof(modelType))
        };
    }

    public System.Type GetRequiredType()
    {
        return typeof(Type);
    }

    public enum Type
    {
        Home,
        Reports,
        Profile,
        Accounts,
        Documentation,
        Login,
        Registration
    }
}