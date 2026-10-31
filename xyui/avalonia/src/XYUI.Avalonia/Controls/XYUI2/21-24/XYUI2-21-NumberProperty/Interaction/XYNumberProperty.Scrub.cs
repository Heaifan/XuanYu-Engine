using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberProperty
{
    const double ScrubDipPerStep = 4;
    double _scrubStartX;
    double _scrubStartValue;
    IPointer? _scrubPointer;
    bool _scrubArmed;

    internal void OnLabelPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsEnabled || IsReadOnly || e.GetCurrentPoint(LabelPart).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        _scrubPointer = e.Pointer; _scrubStartX = e.GetPosition(LabelPart).X; _scrubStartValue = Value; _scrubArmed = true;
    }
    internal void OnLabelMoved(object? sender, PointerEventArgs e)
    {
        if (!_scrubArmed || _scrubPointer != e.Pointer || ValueFieldPart is null) return;
        var delta = e.GetPosition(LabelPart).X - _scrubStartX;
        if (Math.Abs(delta) < ScrubDipPerStep) return;
        e.Pointer.Capture(LabelPart); Classes.Set("xyui-number-property-scrubbing", true);
        ValueFieldPart.Value = CalculateScrubValue(_scrubStartValue, delta, DecimalPlaces); e.Handled = true;
    }
    internal void OnLabelReleased(object? sender, PointerReleasedEventArgs e) { if (_scrubPointer != e.Pointer) return; e.Pointer.Capture(null); ClearScrub(); }
    internal void OnLabelCaptureLost(object? sender, PointerCaptureLostEventArgs e) => ClearScrub();
    void ClearScrub() { _scrubPointer = null; _scrubArmed = false; Classes.Set("xyui-number-property-scrubbing", false); }
    internal static double CalculateScrubValue(double startValue, double delta, int decimalPlaces) => startValue + Math.Round(delta / ScrubDipPerStep) * Math.Pow(10, -Math.Max(0, decimalPlaces));
}
