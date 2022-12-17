using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models;
using Domain.Services;
using HandyControl.Controls;
using HandyControl.Data;
using WPF.Commands;
using WPF.State.Authenticators;

namespace WPF.ViewModels;

public partial class ReportsViewModel : ViewModelBase, IAccessibleViewModel
{
    [ObservableProperty] private ObservableCollection<Report> _reports = new();

    private static int PageSize => 9;
    [ObservableProperty] private int _maxPages = 5;
    [ObservableProperty] private int _currentPage = 1;

    #region ReportControl

    private Report? _selectedReport;
    [ObservableProperty] private string? _agentComment;
    [ObservableProperty] private Report.State? _reportState;

    public Report? SelectedReport
    {
        get => _selectedReport;
        set
        {
            _selectedReport = value;
            AgentComment = _selectedReport?.AgentComment;
            ReportState = _selectedReport?.ReportState;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Services

    private readonly IReportService _reportService;
    private readonly IAuthenticator _authenticator;

    #endregion

    public int AccessLevel => 1;

    #region Commands

    public ICommand LoadReports => new AsyncRelayCommand<int>(async page =>
    {
        MaxPages = await _reportService.GetPagesCount(PageSize);
        var reports = await _reportService.GetPage(page, PageSize);
        Reports = new ObservableCollection<Report>(reports);
    });

    public ICommand SaveReport => new AsyncRelayCommand<Report>(async report =>
    {
        report.ReportState = ReportState ?? report.ReportState;
        report.AgentComment = AgentComment;
        if (report.ReportState != Report.State.Sent)
            report.Agent = _authenticator.CurrentAccount;
        SelectedReport = await _reportService.Update(report.Id, report);
        Growl.Success(new GrowlInfo
        {
            Message = "Заявка обновлена",
            WaitTime = 4,
            Token = "GrowlMsg"
        });
    }, report => ReportState != null &&
                 (report!.Agent == null || report.Agent.Equals(_authenticator.CurrentAccount)));

    #endregion

    public ReportsViewModel(IReportService reportService, IAuthenticator authenticator)
    {
        _reportService = reportService;
        _authenticator = authenticator;
        LoadReports.Execute(1);
    }
}