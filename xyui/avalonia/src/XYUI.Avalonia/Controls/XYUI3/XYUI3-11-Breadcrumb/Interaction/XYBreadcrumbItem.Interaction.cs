using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBreadcrumbItem
{
    public event EventHandler? Invoked;
    public event EventHandler? DropdownRequested;
    internal event EventHandler<Key>? NavigationRequested;

    void InitializeInteraction() { PointerPressed += OnPointerPressed; KeyDown += OnKeyDown; }

    public void Invoke()
    {
        if (!IsEnabled) return;
        if (IsCollapsed || HasDropdown) DropdownRequested?.Invoke(this, EventArgs.Empty);
        else Invoked?.Invoke(this, EventArgs.Empty);
    }

    void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Focus(); Invoke(); e.Handled = true;
    }

    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Left or Key.Right or Key.Down) { NavigationRequested?.Invoke(this, e.Key); e.Handled = true; return; }
        if (e.Key is not (Key.Enter or Key.Space)) return;
        Invoke(); e.Handled = true;
    }
}
