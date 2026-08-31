using System.Windows.Input;

namespace XYUI.Avalonia.Tests;

sealed class SplitButtonCountingCommand : ICommand
{
    public int Executions { get; private set; }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => Executions++;
    public event EventHandler? CanExecuteChanged { add { } remove { } }
}
