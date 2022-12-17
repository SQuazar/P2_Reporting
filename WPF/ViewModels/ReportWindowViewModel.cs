using System;

namespace WPF.ViewModels;

public class ReportWindowViewModel : ViewModelBase
{
    public string Sender { get; }

    public string Title { get; }

    public string Status { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public string? AgentComment { get; }

    public ReportWindowViewModel(string sender, string title, string status, DateTime dateTime, string? description,
        string? agentComment)
    {
        Sender = sender;
        Title = title;
        Status = status;
        DateTime = dateTime;
        Description = description;
        AgentComment = agentComment;
    }
}