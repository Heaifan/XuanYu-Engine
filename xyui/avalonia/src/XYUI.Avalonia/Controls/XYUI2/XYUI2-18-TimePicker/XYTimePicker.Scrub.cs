using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYTimePicker
{
    const double ScrubDipPerStep = 4;
    double _scrubStartX; TimeOnly _scrubStartTime; IPointer? _scrubPointer;
    internal void OnSurfacePointerPressed(object? sender, PointerEventArgs e) { if (TrySegmentAt(e.GetPosition(this), out var segment)) BeginScrub(segment, e); else if (e.GetPosition(this).X < Bounds.Width - 32) BeginScrub(ActiveSegment, e); }
    void BeginScrub(XYTimeSegment segment, PointerEventArgs e)
    {
        if (!IsEnabled) return; ActivateSegment(segment); _scrubPointer = e.Pointer; _scrubStartX = e.GetPosition(this).X; _scrubStartTime = Time; ScrubSegment = segment; IsScrubArmed = true; IsScrubbing = false;
    }
    void OnTimePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!IsScrubArmed && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && TrySourceSegment(e, out var segment)) BeginScrub(segment, e);
        if (!IsScrubArmed || _scrubPointer != e.Pointer) return; var delta = e.GetPosition(this).X - _scrubStartX; if (!IsScrubbing && Math.Abs(delta) < ScrubDipPerStep) return; if (!IsScrubbing) { IsScrubbing = true; e.Pointer.Capture(this); } var steps = (int)Math.Round(delta / ScrubDipPerStep); Time = _scrubStartTime; SetSegment(ScrubSegment, GetStartValue(ScrubSegment) + steps); Classes.Set("xyui-time-scrubbing", true); if (ScrubIndicatorPart is not null) ScrubIndicatorPart.IsVisible = true; e.Handled = true;
    }
    void OnTimePointerReleased(object? sender, PointerEventArgs e) { if (_scrubPointer != e.Pointer) return; CommitScrub(); }
    void OnTimePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) { if (IsScrubArmed) CancelScrub(); }
    int GetStartValue(XYTimeSegment segment) => segment switch { XYTimeSegment.Hour => _scrubStartTime.Hour, XYTimeSegment.Minute => _scrubStartTime.Minute, _ => _scrubStartTime.Second };
    bool TrySourceSegment(PointerEventArgs e, out XYTimeSegment segment) { if (e.Source is Button button) { foreach (var pair in SegmentButtons) if (pair.Value == button) { segment = pair.Key; return true; } } segment = default; return false; }
    bool TrySegmentAt(Point point, out XYTimeSegment segment) { foreach (var pair in SegmentButtons) { var origin = pair.Value.TranslatePoint(new Point(0, 0), this); if (origin is not null && new Rect(origin.Value, pair.Value.Bounds.Size).Contains(point)) { segment = pair.Key; return true; } } segment = default; return false; }
    void CommitScrub() { var pointer = _scrubPointer; ClearScrub(); pointer?.Capture(null); }
    internal void CancelScrub() { if (!IsScrubArmed && !IsScrubbing) return; Time = _scrubStartTime; var pointer = _scrubPointer; ClearScrub(); pointer?.Capture(null); }
    void ClearScrub() { _scrubPointer = null; IsScrubArmed = false; IsScrubbing = false; Classes.Set("xyui-time-scrubbing", false); if (ScrubIndicatorPart is not null) ScrubIndicatorPart.IsVisible = false; }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { CancelScrub(); base.OnDetachedFromVisualTree(e); }
}

sealed class XYTimeSegmentButton : Button
{
    internal Action<PointerEventArgs>? Pressed { get; init; }
    protected override void OnPointerPressed(PointerPressedEventArgs e) { Pressed?.Invoke(e); base.OnPointerPressed(e); }
}
