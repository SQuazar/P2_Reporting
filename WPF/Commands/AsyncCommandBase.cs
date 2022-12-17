using System;
using System.Threading.Tasks;

namespace WPF.Commands;

public abstract class AsyncCommandBase : CommandBase
{
    public bool IsExecuting { get; private set; }

    public override bool CanExecute(object? parameter)
    {
        return !IsExecuting && base.CanExecute(parameter);
    }

    public override async void Execute(object? parameter)
    {
        IsExecuting = true;
        await ExecuteAsync(parameter);
        IsExecuting = false;
    }

    public abstract Task ExecuteAsync(object? parameter);
}