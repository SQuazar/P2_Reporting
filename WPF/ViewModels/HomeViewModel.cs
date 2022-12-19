using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models;
using Domain.Services;
using HandyControl.Controls;
using HandyControl.Data;
using WPF.Attributes;
using WPF.Commands;
using WPF.Controls;
using WPF.State.Authenticators;

namespace WPF.ViewModels;

[ProtectedViewModel(AccessLevel.User)]
public partial class HomeViewModel : ViewModelBase
{
    private readonly IAuthenticator _authenticator;
    private readonly IReportService _reportService;

    [ObservableProperty] private ObservableCollection<Report> _reports = new();

    [ObservableProperty] private Report? _selectedReport;
    [ObservableProperty] private string? _reportTitle;
    [ObservableProperty] private string? _reportDescription;
    
    public int UserTotalReports { get; private set; }
    public int UserSentReports { get; private set; }
    public int UserInProgressReports { get; private set; }

    public ICommand SendReport => new AsyncRelayCommand(async _ =>
    {
        Growl.Info(new GrowlInfo
        {
            Message = $"Отправка заявки \"{ReportTitle}\"",
            Token = "GrowlMsg",
            WaitTime = 3
        });

        var report = await _reportService.Send(_authenticator.CurrentAccount!, ReportTitle!, ReportDescription);
        report.Sender = _authenticator.CurrentAccount!;
        _reports.Add(report);
        var list = new List<Report>(_reports);
        list.Sort((r1, r2) => r2.ReportDate.CompareTo(r1.ReportDate));
        Reports = new ObservableCollection<Report>(list);
        UserTotalReports += 1;
        UserSentReports += 1;

        Growl.Success(new GrowlInfo
        {
            Message = $"Заявка #{report.Id} отправлена",
            Token = "GrowlMsg",
            WaitTime = 3
        });

        ReportTitle = null;
        ReportDescription = null;

        OnPropertyChanged(nameof(UserTotalReports));
        OnPropertyChanged(nameof(UserSentReports));
    }, _ => !string.IsNullOrEmpty(ReportTitle));

    private ICommand LoadReports => new AsyncRelayCommand(async _ =>
    {
        if (_authenticator.CurrentAccount == null) return;
        var reports = await _reportService.GetAllBySender(_authenticator.CurrentAccount);
        var list = reports.ToList();
        list.Sort((r1, r2) => r2.ReportDate.CompareTo(r1.ReportDate));
        Reports = new ObservableCollection<Report>(list);
        UserTotalReports = Reports.Count;
        UserSentReports = Reports.Count(r => r.ReportState == Report.State.Sent);
        UserInProgressReports = Reports.Count(r => r.ReportState == Report.State.InProgress);
        OnPropertyChanged(nameof(Report));
        OnPropertyChanged(nameof(UserTotalReports));
        OnPropertyChanged(nameof(UserSentReports));
        OnPropertyChanged(nameof(UserInProgressReports));
    });

    public ICommand OpenReportWindow => new RelayCommand<Report>(report =>
    {
        var window = new ReportView(new ReportWindowViewModel
            (
                report!.Sender.FullName,
                report.Title,
                report.ReportState.Localized,
                report.ReportDate,
                report.Description,
                report.AgentComment)
        );
        window.Show();
    });

    public HomeViewModel(IAuthenticator authenticator, IReportService reportService)
    {
        _authenticator = authenticator;
        _reportService = reportService;
        LoadReports.Execute(null);
    }
}