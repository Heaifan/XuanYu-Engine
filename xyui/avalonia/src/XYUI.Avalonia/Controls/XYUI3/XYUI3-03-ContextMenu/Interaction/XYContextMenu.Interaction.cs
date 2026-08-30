using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYContextMenu
{
    Popup? _popup;
    public bool IsOpen { get; private set; }
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public void AttachTo(Control source) { source.PointerPressed -= OnSourcePointerPressed; source.PointerPressed += OnSourcePointerPressed; }
    public void Open(Control target)
    {
        if (!IsEnabled) return; Close(); Menu.ClearSelection(); Menu.Open(); _host.Children.Remove(_surface); _popup = new Popup { PlacementTarget = target, Placement = PlacementMode.Pointer, IsLightDismissEnabled = true, Child = _surface };
        _popup.Closed += OnPopupClosed; IsOpen = true; _popup.IsOpen = true; Opened?.Invoke(this, EventArgs.Empty);
    }
    public void Close()
    {
        if (!IsOpen && _popup is null && Menu.SelectedItem is null) return; IsOpen = false; if (_popup is not null) { _popup.Closed -= OnPopupClosed; _popup.IsOpen = false; _popup.Child = null; _popup = null; }
        Menu.Close(); if (!_host.Children.Contains(_surface)) _host.Children.Insert(0, _surface); Closed?.Invoke(this, EventArgs.Empty);
    }
    void OnPopupClosed(object? sender, EventArgs e) => Close();
    void OnSourcePointerPressed(object? sender, PointerPressedEventArgs e)
    { if (e.GetCurrentPoint((Visual)sender!).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed) { Open((Control)sender!); e.Handled = true; } }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape) { Close(); e.Handled = true; return; } base.OnKeyDown(e); }
}
