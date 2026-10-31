using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationItem
{
    bool _pointerHooked;
    public event EventHandler? Selected;
    public event EventHandler? Invoked;
    void HookInteraction()
    {
        if (_pointerHooked) return;
        Focusable = true;
        PointerPressed += (_, e) => { if (IsEnabled) { IsSelected = true; Selected?.Invoke(this, EventArgs.Empty); Invoked?.Invoke(this, EventArgs.Empty); e.Handled = true; } };
        KeyDown += (_, e) => { if (IsEnabled && e.Key is Key.Enter or Key.Space) { IsSelected = true; Selected?.Invoke(this, EventArgs.Empty); Invoked?.Invoke(this, EventArgs.Empty); e.Handled = true; } };
        _pointerHooked = true;
    }
}
