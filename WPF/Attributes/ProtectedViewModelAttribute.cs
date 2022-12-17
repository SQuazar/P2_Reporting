using System;
using Domain.Models;

namespace WPF.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ProtectedViewModelAttribute : Attribute
{
    public AccessLevel AccessLevel { get; }

    public ProtectedViewModelAttribute(AccessLevel accessLevel)
    {
        AccessLevel = accessLevel;
    }
}