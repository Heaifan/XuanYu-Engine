using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYTimePicker
{
    const double ScrubDipPerStep = 4;
    double _scrubStartX; TimeOnly _scrubStartTime; IPointer? _scrubPointer;
    internal void OnSurfacePointerPressed(object? sender, PointerEventArgs e) { if (TrySegmentAt(e.GetPosition(this), out var segment)) BeginScrub(segment, e); else if (e.GetPosition(this).X < Bounds.Width - 32) BeginScrub(ActiveSegment, e); }
    void BeginScrub(XYTimeSegment segment, PointerEventArgs e)
    {
        if (!IsEnabled) return; ActivateSegment(segment); _scrubPointer = e.Pointer; _scrubStartX = e.GetPosition(this).X; _scrubStartTime = Time; ScrubSegment = segment; IsScrubArmed = true; IsScrubbing = false; PointerActionPending = true;
    }
    void OnTimePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!IsScrubArmed || _scrubPointer != e.Pointer) return; var delta = e.GetPosition(this).X - _scrubStartX; if (!IsScrubbing && Math.Abs(delta) < ScrubDipPerStep) return; if (!IsScrubbing) { IsScrubbing = true; e.Pointer.Capture(this); } var steps = (int)Math.Round(delta / ScrubDipPerStep); Time = _scrubStartTime; SetSegment(ScrubSegment, GetStartValue(ScrubSegment) + steps); Classes.Set("xyui-time-scrubbing", true); if (ScrubIndicatorPart is not null) ScrubIndicatorPart.IsVisible = true; e.Handled = true;
    }
    void OnTimePointerReleased(object? sender, PointerEventArgs e) { if (_scrubPointer != e.Pointer) return; if (IsScrubbing) CommitScrub(); else { var segment = ScrubSegment; ClearScrub(); e.Pointer.Capture(null); ActivateSegment(segment); OpenTimePopup(segment); } Dispatcher.UIThread.Post(() => PointerActionPending = false); }
    void OnTimePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) { if (IsScrubArmed) CancelScrub(); }
    int GetStartValue(XYTimeSegment segment) => segment switch { XYTimeSegment.Hour => _scrubStartTime.Hour, XYTimeSegment.Minute => _scrubStartTime.Minute, _ => _scrubStartTime.Second };
    bool TrySegmentAt(Point point, out XYTimeSegment segment) { foreach (var pair in SegmentButtons) { var origin = pair.Value.TranslatePoint(new Point(0, 0), this); if (origin is not null && new Rect(origin.Value, pair.Value.Bounds.Size).Contains(point)) { segment = pair.Key; return true; } } segment = default; return false; }
    void CommitScrub() { CommitSegmentEdit(); var pointer = _scrubPointer; ClearScrub(); pointer?.Capture(null); }
    internal void CancelScrub() { if (!IsScrubArmed && !IsScrubbing) return; Time = _scrubStartTime; CancelSegmentEdit(); var pointer = _scrubPointer; ClearScrub(); pointer?.Capture(null); }
    internal bool ConsumePointerAction() { if (!PointerActionPending) return false; PointerActionPending = false; return true; }
    void ClearScrub() { _scrubPointer = null; IsScrubArmed = false; IsScrubbing = false; Classes.Set("xyui-time-scrubbing", false); if (ScrubIndicatorPart is not null) ScrubIndicatorPart.IsVisible = false; }
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnWindowDeactivated; _hostWindow.Closed += OnWindowClosed; } }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { CancelScrub(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnWindowDeactivated; _hostWindow.Closed -= OnWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e); }
    void OnDeactivated(object? sender, ActivatedEventArgs e) => CancelScrub(); void OnWindowDeactivated(object? sender, EventArgs e) => CancelScrub(); void OnWindowClosed(object? sender, EventArgs e) => CancelScrub();
}
