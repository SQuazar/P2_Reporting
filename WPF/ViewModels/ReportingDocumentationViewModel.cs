using System;

namespace WPF.ViewModels;

public class ReportingDocumentationViewModel : ViewModelBase, IAccessibleViewModel
{
    public int AccessLevel => Convert.ToInt32(Domain.Models.AccessLevel.Accountant);
}