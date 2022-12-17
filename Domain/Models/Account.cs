#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Domain.Models;

[Table("account")]
public partial class Account : DomainObject
{
    public static readonly Account Empty = new()
    {
        Username = "",
        Firstname = "",
        Surname = "",
        PasswordHash = "",
        Roles = new List<Role>()
    };

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FullName))]
    private string _username = null!;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FullName))]
    private string _firstname = null!;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FullName))]
    private string _surname = null!;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FullName))]
    private string? _middleName;

    [ObservableProperty] private string _passwordHash = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AccessLevel), nameof(PriorityRole), nameof(DescendedSortedRoles))]
    private List<Role> _roles = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalReports), nameof(SentReports), nameof(InProgressReports))]
    private List<Report> _reports = null!;
    
    [ObservableProperty] private List<Report>? _attachedReports;
    [NotMapped] public int AccessLevel => Roles.Select(r => r.AccessLevel).Max();

    [NotMapped] public string FullName => $"{Surname} {Firstname} {MiddleName}";

    [NotMapped] public Role PriorityRole => Roles.MaxBy(r => r.AccessLevel)!;

    [NotMapped] public List<Role> DescendedSortedRoles => Roles.OrderByDescending(r => r.AccessLevel).ToList();

    [NotMapped] public int TotalReports => Reports.Count;
    [NotMapped] public int SentReports => Reports.Count(r => r.ReportState == Report.State.Sent);
    [NotMapped] public int InProgressReports => Reports.Count(r => r.ReportState == Report.State.InProgress);

    public override string ToString()
    {
        return $"{Username} Roles: [{string.Join(", ", Roles)}]";
    }

    public override bool Equals(object? obj)
    {
        if (obj == this) return true;
        if (obj is not Account that) return false;
        return that.Id == Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}