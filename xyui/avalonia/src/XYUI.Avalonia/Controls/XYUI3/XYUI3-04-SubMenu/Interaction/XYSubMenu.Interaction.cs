using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSubMenu
{
    void InitializeInteraction() { Focusable = true; KeyDown += OnKeyDown; }
    public void Open() { if (IsOpen) return; IsOpen = true; ChildMenu.IsVisible = true; ChildMenu.Open(); Opened?.Invoke(this, EventArgs.Empty); }
    public void Close() { if (!IsOpen) return; IsOpen = false; ChildMenu.IsVisible = false; ChildMenu.Close(); ParentMenu.ClearSelection(); Closed?.Invoke(this, EventArgs.Empty); }
    void OnTriggerRequested(object? sender, EventArgs e) => Open();
    void OnKeyDown(object? sender, KeyEventArgs e)
    { if (e.Key == Key.Right) { Open(); e.Handled = true; } else if (e.Key is Key.Left or Key.Escape) { Close(); e.Handled = true; } }
}
