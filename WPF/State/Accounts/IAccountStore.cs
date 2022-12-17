using System;
using Domain.Models;

namespace WPF.State.Accounts;

public interface IAccountStore
{
    Account? CurrentAccount { get; set; }
    event Action StateChanged;
}