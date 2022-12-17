using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("role")]
public class Role : DomainObject
{
    public string Name { get; set; } = null!;
    public int AccessLevel { get; set; }
    public List<Account> Accounts { get; set; } = null!;

    public override bool Equals(object obj)
    {
        if (obj == this) return true;
        if (obj is not Role that) return false;
        return that.Id == Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public override string ToString()
    {
        return $"{Name} A: {AccessLevel}";
    }
}