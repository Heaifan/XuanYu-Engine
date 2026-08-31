using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuItem
{
    public event EventHandler? Invoked;
    public event EventHandler? SelectionRequested;
    public event EventHandler? SubMenuRequested;
    public Action? Command { get; set; }
    public bool IsSubMenuOpen { get; private set; }
    void InitializeInteraction()
    {
        Focusable = true; PointerEntered += (_, _) => IsHovered = true; PointerExited += (_, _) => IsHovered = false;
        PointerPressed += OnPointerPressed; KeyDown += OnKeyDown;
    }
    public bool Activate()
    {
        if (!IsEnabled) return false;
        if (IsSelected)
        {
            IsSelected = false; IsSubMenuOpen = false; Command?.Invoke(); Invoked?.Invoke(this, EventArgs.Empty); return true;
        }
        IsSelected = true; SelectionRequested?.Invoke(this, EventArgs.Empty);
        if (HasSubMenu) { IsSubMenuOpen = true; SubMenuRequested?.Invoke(this, EventArgs.Empty); return true; }
        return true;
    }
    internal void ClearInteractionState() { IsSelected = false; IsSubMenuOpen = false; }
    void OnPointerPressed(object? sender, PointerPressedEventArgs e) { if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) { Activate(); e.Handled = true; } }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key is Key.Enter or Key.Space) { Activate(); e.Handled = true; } }
}
