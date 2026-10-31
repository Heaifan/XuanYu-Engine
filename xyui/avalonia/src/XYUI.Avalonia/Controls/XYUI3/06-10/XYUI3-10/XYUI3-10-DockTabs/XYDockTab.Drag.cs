using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYDockTab
{
    bool _dragging;
    Point _start;
    internal event EventHandler<double>? DropRequested;
    internal event EventHandler<double>? DragMoved;
    internal event EventHandler? DragStarted;
    internal event EventHandler? DragCanceled;
    public bool IsDragging => _dragging;

    void InitializeInteraction()
    {
        _grip.IsHitTestVisible = false; _gripHitArea.Cursor = new Cursor(StandardCursorType.SizeWestEast);
        _gripHitArea.PointerPressed += OnGripPressed; _gripHitArea.PointerMoved += OnGripMoved;
        _gripHitArea.PointerReleased += OnGripReleased; _gripHitArea.PointerCaptureLost += OnGripCaptureLost; KeyDown += OnKeyDown;
    }

    void OnGripPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(_gripHitArea).Properties.IsLeftButtonPressed) return;
        _start = e.GetPosition(this); _dragging = false; e.Pointer.Capture(_gripHitArea); e.Handled = true;
    }

    void OnGripMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragging && e.Pointer.Captured != _gripHitArea) return;
        var delta = e.GetPosition(this) - _start; if (!_dragging && Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y) < XyuiCompactNavigationTokens.DragThreshold) return;
        if (!_dragging) { _dragging = true; Classes.Set("xyui-dock-tab-dragging", true); DragStarted?.Invoke(this, EventArgs.Empty); }
        var owner = this.FindAncestorOfType<XYDockTabs>(); if (owner is not null) DragMoved?.Invoke(this, e.GetPosition(owner).X); e.Handled = true;
    }

    void OnGripReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured != _gripHitArea) return;
        var owner = this.FindAncestorOfType<XYDockTabs>();
        if (_dragging && owner is not null) DropRequested?.Invoke(this, e.GetPosition(owner).X);
        ResetDrag(e.Pointer); e.Handled = true;
    }

    void OnGripCaptureLost(object? sender, PointerCaptureLostEventArgs e) => CancelDrag(e.Pointer);
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Escape && _dragging) { CancelDrag(null); e.Handled = true; } }
    void CancelDrag(IPointer? pointer)
    { if (!_dragging) return; _dragging = false; Classes.Set("xyui-dock-tab-dragging", false); DragCanceled?.Invoke(this, EventArgs.Empty); pointer?.Capture(null); }
    void ResetDrag(IPointer pointer) { _dragging = false; Classes.Set("xyui-dock-tab-dragging", false); pointer.Capture(null); }
}
