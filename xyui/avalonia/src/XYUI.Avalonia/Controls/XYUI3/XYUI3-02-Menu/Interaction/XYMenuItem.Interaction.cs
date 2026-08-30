using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuItem
{
    public event EventHandler? Invoked;
    public event EventHandler? SubMenuRequested;
    public Action? Command { get; set; }
    public bool IsSubMenuOpen { get; private set; }
    void InitializeInteraction()
    {
        Focusable = true; PointerEntered += (_, _) => IsHovered = true; PointerExited += (_, _) => IsHovered = false;
        PointerReleased += OnPointerReleased; KeyDown += OnKeyDown;
    }
    public bool Activate()
    {
        if (!IsEnabled) return false;
        if (HasSubMenu) { IsSubMenuOpen = true; SubMenuRequested?.Invoke(this, EventArgs.Empty); return true; }
        Command?.Invoke(); Invoked?.Invoke(this, EventArgs.Empty); return true;
    }
    void OnPointerReleased(object? sender, PointerReleasedEventArgs e) { if (e.InitialPressMouseButton == MouseButton.Left) { Activate(); e.Handled = true; } }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key is Key.Enter or Key.Space) { Activate(); e.Handled = true; } }
}
