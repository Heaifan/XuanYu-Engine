using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYDockTab
{
    internal event EventHandler<double>? DropRequested;

    void InitializeInteraction()
    {
        _grip.Cursor = new Cursor(StandardCursorType.SizeWestEast);
        _grip.PointerPressed += OnGripPressed;
        _grip.PointerReleased += OnGripReleased;
    }

    void OnGripPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(_grip).Properties.IsLeftButtonPressed) return;
        e.Pointer.Capture(_grip); e.Handled = true;
    }

    void OnGripReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured != _grip) return;
        var owner = this.FindAncestorOfType<XYDockTabs>();
        if (owner is not null) DropRequested?.Invoke(this, e.GetPosition(owner).X);
        e.Pointer.Capture(null); e.Handled = true;
    }
}
