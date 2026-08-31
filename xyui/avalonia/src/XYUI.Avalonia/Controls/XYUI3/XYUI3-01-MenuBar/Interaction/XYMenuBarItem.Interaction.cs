using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBarItem
{
    void InitializeInteraction()
    {
        Focusable = true; PointerEntered += (_, _) => IsHovered = true; PointerExited += (_, _) => IsHovered = false;
        PointerPressed += OnPointerPressed; KeyDown += OnKeyDown;
    }
    public void Activate() { if (IsEnabled) Activated?.Invoke(this, EventArgs.Empty); }
    void OnPointerPressed(object? sender, PointerPressedEventArgs e) { if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) { Activate(); e.Handled = true; } }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key is Key.Enter or Key.Down) { Activate(); e.Handled = true; } }
}
