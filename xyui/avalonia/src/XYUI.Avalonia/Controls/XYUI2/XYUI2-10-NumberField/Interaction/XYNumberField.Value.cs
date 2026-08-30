using System.Globalization;
using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberField
{
    bool _syncingText;
    internal string FormatValue(double value) => value.ToString($"F{DecimalPlaces}", CultureInfo.InvariantCulture);
    internal bool TryParseText(string? text, out double value)
    {
        return double.TryParse(text?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
    internal void SyncText() { _syncingText = true; Text = FormatValue(Value); _syncingText = false; }
    internal bool CommitText()
    {
        if (!TryParseText(Text, out var value)) { IsError = true; SyncText(); return false; }
        Value = value; IsError = false; SyncText(); return true;
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty || change.Property == DecimalPlacesProperty) SyncText();
        if (change.Property == MinimumProperty || change.Property == MaximumProperty) Value = Math.Clamp(Value, Minimum, Maximum);
    }
    void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_syncingText) return;
        IsError = !TryParseText(Text, out _);
    }
}
