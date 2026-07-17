using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class UiWin : Window
{
    public UiWin()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        (DataContext as UiVm)?.CancelInteractionFromWindowClosing();
        base.OnClosing(e);
    }

    void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape();
        e.Handled = true;
    }
}
