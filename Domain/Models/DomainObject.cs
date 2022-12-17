using CommunityToolkit.Mvvm.ComponentModel;

namespace Domain.Models;

public class DomainObject : ObservableObject
{
    public int Id { get; set; }
}