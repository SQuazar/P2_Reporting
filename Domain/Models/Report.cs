#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Domain.Models;

[Table("report")]
public partial class Report : DomainObject
{
    #region StaticMembers

    public static readonly Report Empty = new() { Sender = Account.Empty, Title = "" };

    #endregion

    [ObservableProperty] private Account _sender = null!;
    [ObservableProperty] private int _senderId;
    [ObservableProperty] private string _title = null!;
    [ObservableProperty] private string? _description;
    [ObservableProperty] private State _reportState = State.Sent;
    [ObservableProperty] private Account? _agent;
    [ObservableProperty] private int? _agentId;
    [ObservableProperty] private string? _agentComment;
    [ObservableProperty] private DateTime _reportDate = DateTime.Now;

    public override bool Equals(object? obj)
    {
        if (obj == this) return true;
        if (obj is not Report that) return false;
        return that.Id == Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public class State
    {
        public static readonly State Sent = new("Sent", "Отправлено");
        public static readonly State InProgress = new("InProgress", "Выполняется");
        public static readonly State Completed = new("Completed", "Выполнено");
        public static readonly State Invalid = new("Invalid", "Отклонён");

        public string Name { get; }

        public string Localized { get; }

        private State(string name, string localized)
        {
            Name = name;
            Localized = localized;
        }

        public override string ToString()
        {
            return Localized;
        }

        public static IEnumerable<State> Values()
        {
            return typeof(State).GetFields().Select(fieldInfo => fieldInfo.GetValue(null) as State).ToArray()!;
        }

        public static State? ValueOf(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return typeof(State).GetFields()
                .Select(fieldInfo => fieldInfo.GetValue(null) as State)
                .FirstOrDefault(state => state?.Name == name);
        }
    }
}