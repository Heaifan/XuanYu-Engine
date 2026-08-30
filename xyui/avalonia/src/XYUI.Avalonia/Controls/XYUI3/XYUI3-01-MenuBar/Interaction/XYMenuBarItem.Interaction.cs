using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBarItem
{
    void InitializeInteraction()
    {
        Focusable = true; PointerEntered += (_, _) => IsHovered = true; PointerExited += (_, _) => IsHovered = false;
        PointerReleased += OnPointerReleased; KeyDown += OnKeyDown;
    }
    public void Activate() { if (IsEnabled) Activated?.Invoke(this, EventArgs.Empty); }
    void OnPointerReleased(object? sender, PointerReleasedEventArgs e) { if (e.InitialPressMouseButton == MouseButton.Left) { Activate(); e.Handled = true; } }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key is Key.Enter or Key.Down) { Activate(); e.Handled = true; } }
}
