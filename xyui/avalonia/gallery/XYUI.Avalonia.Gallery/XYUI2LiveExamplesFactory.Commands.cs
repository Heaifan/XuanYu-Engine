using System.Windows.Input;

namespace XYUI.Avalonia.Gallery;

sealed class XYUI2GalleryCommand : ICommand
{
    readonly Action _execute;
    public XYUI2GalleryCommand(Action execute) => _execute = execute;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute();
    public event EventHandler? CanExecuteChanged { add { } remove { } }
}
