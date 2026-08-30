using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberField
{
    const double ScrubDipPerStep = 4;
    double _scrubStartX;
    IPointer? _scrubPointer;

    void OnNumberPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        if (!IsScrubEnabled || IsReadOnly || !IsEnabled || !IsPointerInsideValueHost(e) || point.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed) return;
        CommitText(); _scrubPointer = e.Pointer; _scrubStartX = e.GetPosition(this).X; ScrubStartValue = Value; IsScrubArmed = true; IsScrubbing = false;
    }

    void OnNumberPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!IsScrubArmed || _scrubPointer != e.Pointer) return;
        var delta = e.GetPosition(this).X - _scrubStartX;
        if (!IsScrubbing && Math.Abs(delta) < ScrubDipPerStep) return;
        if (!IsScrubbing) BeginScrub(e.Pointer);
        Value = ScrubStartValue + Math.Round(delta / ScrubDipPerStep) * PrecisionStep; e.Handled = true;
    }

    void OnNumberPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_scrubPointer != e.Pointer) return;
        if (IsScrubbing) CommitScrub(); else ClearScrub();
    }

    bool IsPointerInsideValueHost(PointerEventArgs e)
    {
        ValueHost ??= this.GetVisualDescendants().OfType<Control>().FirstOrDefault(x => x.Name == "PART_ValueHost");
        if (ValueHost is null) return false;
        var origin = ValueHost.TranslatePoint(new Point(0, 0), this);
        return origin is not null && new Rect(origin.Value, ValueHost.Bounds.Size).Contains(e.GetPosition(this));
    }
    void BeginScrub(IPointer pointer) { IsScrubbing = true; pointer.Capture(this); }
    void CommitScrub() { _scrubPointer?.Capture(null); ClearScrub(); SyncText(); }
    internal void CancelScrub() { if (!IsScrubArmed && !IsScrubbing) return; Value = ScrubStartValue; _scrubPointer?.Capture(null); ClearScrub(); SyncText(); }
    void ClearScrub() { _scrubPointer = null; IsScrubArmed = false; IsScrubbing = false; }
}
