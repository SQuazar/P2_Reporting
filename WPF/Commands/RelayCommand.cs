using System;
using System.Threading.Tasks;

namespace WPF.Commands;

public class RelayCommand<T> : CommandBase
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        if (parameter is not T p) return false;
        return base.CanExecute(parameter) && (_canExecute == null || _canExecute(p));
    }

    public override void Execute(object? parameter)
    {
        if (parameter is not T p) return;
        _execute.Invoke(p);
    }
}

public class AsyncRelayCommand<T> : AsyncCommandBase
{
    private readonly Action<T> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public AsyncRelayCommand(Action<T> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        if (parameter is not T p) return false;
        return base.CanExecute(parameter) && (_canExecute == null || _canExecute(p));
    }

    public override Task ExecuteAsync(object? parameter)
    {
        if (parameter is not T p) return Task.CompletedTask;
        _execute(p);
        return Task.CompletedTask;
    }
}

public class RelayCommand : CommandBase
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter) && (_canExecute == null || _canExecute(parameter));
    }

    public override void Execute(object? parameter)
    {
        _execute(parameter);
    }
}

public class AsyncRelayCommand : AsyncCommandBase
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public AsyncRelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter) && (_canExecute == null || _canExecute(parameter));
    }

    public override Task ExecuteAsync(object? parameter)
    {
        _execute(parameter);
        return Task.CompletedTask;
    }
}