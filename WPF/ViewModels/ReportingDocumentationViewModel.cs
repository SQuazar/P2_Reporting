using Domain.Models;
using WPF.Attributes;

namespace WPF.ViewModels;

[ProtectedViewModel(AccessLevel.Accountant)]
public class ReportingDocumentationViewModel : ViewModelBase
{
}