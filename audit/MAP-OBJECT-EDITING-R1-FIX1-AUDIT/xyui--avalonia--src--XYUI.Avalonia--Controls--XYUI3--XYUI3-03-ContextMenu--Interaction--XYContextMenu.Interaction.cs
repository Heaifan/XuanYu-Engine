using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives.PopupPositioning;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYContextMenu
{
    Popup? _popup;
    bool _closing;
    public bool IsOpen { get; private set; }
    public Control? Target { get; private set; }
    public Control? ContextTarget => Target;
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public void AttachTo(Control source) { Target = source; source.PointerPressed -= OnSourcePointerPressed; source.PointerPressed += OnSourcePointerPressed; }
    public void DetachFrom(Control source) { source.PointerPressed -= OnSourcePointerPressed; if (ReferenceEquals(Target, source)) Target = null; }
    public void Open() { if (Target is not null) Open(Target); }
    public void Open(Control target) => OpenAt(target, null);
    public void OpenAt(Control target, Point pointerDip) => OpenAt(target, (Point?)pointerDip);
    void OpenAt(Control target, Point? pointerDip)
    {
        if (!IsEnabled) return; Target = target; Close(); Menu.ClearSelection(); Menu.FocusRestoreTarget = target; XyuiOverlayResourceBridge.Attach(this);
        _popup = new Popup
        {
            PlacementTarget = target,
            Placement = pointerDip is null ? PlacementMode.Pointer : PlacementMode.AnchorAndGravity,
            PlacementRect = pointerDip is { } point ? new Rect(point, new Size(0, 0)) : default,
            PlacementAnchor = PopupAnchor.BottomLeft,
            PlacementGravity = PopupGravity.BottomLeft,
            PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideX |
                PopupPositionerConstraintAdjustment.SlideY | PopupPositionerConstraintAdjustment.FlipY,
            IsLightDismissEnabled = true, Child = this
        };
        _popup.Closed += OnPopupClosed; IsOpen = true; _popup.IsOpen = true; Menu.ApplyOverlayStyling(); Menu.Open(); Opened?.Invoke(this, EventArgs.Empty);
    }
    public void Close()
    {
        if (_closing || !IsOpen && _popup is null && Menu.SelectedItem is null) return; _closing = true; IsOpen = false; if (_popup is not null) { _popup.Closed -= OnPopupClosed; _popup.IsOpen = false; _popup.Child = null; _popup = null; }
        Menu.Close(); _closing = false; Closed?.Invoke(this, EventArgs.Empty);
    }
    void OnMenuClosed(object? sender, EventArgs e) { if (!_closing) Close(); }
    void OnPopupClosed(object? sender, EventArgs e) => Close();
    void OnSourcePointerPressed(object? sender, PointerPressedEventArgs e)
    { if (e.GetCurrentPoint((Visual)sender!).Properties.IsRightButtonPressed) { Open((Control)sender!); e.Handled = true; } }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape) { Close(); e.Handled = true; return; } base.OnKeyDown(e); }
}
