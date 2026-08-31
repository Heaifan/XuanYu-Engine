using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYContextMenu
{
    Popup? _popup;
    bool _closing;
    public bool IsOpen { get; private set; }
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public void AttachTo(Control source) { source.PointerPressed -= OnSourcePointerPressed; source.PointerPressed += OnSourcePointerPressed; }
    public void Open(Control target)
    {
        if (!IsEnabled) return; Close(); Menu.ClearSelection(); _host.Children.Remove(_surface); _popup = new Popup { PlacementTarget = target, Placement = PlacementMode.Pointer, IsLightDismissEnabled = true, Child = _surface };
        _popup.Closed += OnPopupClosed; IsOpen = true; _popup.IsOpen = true; Menu.ApplyOverlayStyling(); Menu.Open(); Opened?.Invoke(this, EventArgs.Empty);
    }
    public void Close()
    {
        if (_closing || !IsOpen && _popup is null && Menu.SelectedItem is null) return; _closing = true; IsOpen = false; if (_popup is not null) { _popup.Closed -= OnPopupClosed; _popup.IsOpen = false; _popup.Child = null; _popup = null; }
        Menu.Close(); if (!_host.Children.Contains(_surface)) _host.Children.Insert(0, _surface); _closing = false; Closed?.Invoke(this, EventArgs.Empty);
    }
    void OnMenuClosed(object? sender, EventArgs e) { if (!_closing) Close(); }
    void OnPopupClosed(object? sender, EventArgs e) => Close();
    void OnSourcePointerPressed(object? sender, PointerPressedEventArgs e)
    { if (e.GetCurrentPoint((Visual)sender!).Properties.IsRightButtonPressed) { Open((Control)sender!); e.Handled = true; } }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape) { Close(); e.Handled = true; return; } base.OnKeyDown(e); }
}
