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
        Deactivated += (_, _) => (DataContext as UiVm)?.CancelInteractionFromWindowDeactivated();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        (DataContext as UiVm)?.CancelInteractionFromWindowClosing();
        base.OnClosing(e);
    }

    void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            (DataContext as UiVm)?.TryUndoFromShortcut();
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape();
        e.Handled = true;
    }
}
