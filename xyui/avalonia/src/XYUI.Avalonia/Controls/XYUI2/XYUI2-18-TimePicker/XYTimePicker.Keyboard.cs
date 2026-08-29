using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYTimePicker
{
    string _digits = "";
    void OnTimeKeyDown(KeyEventArgs e)
    {
        if (!IsEnabled) return;
        if (e.Key is Key.Left or Key.Right) { MoveSegment(e.Key == Key.Right ? 1 : -1); e.Handled = true; return; }
        if (e.Key is Key.Up or Key.Down) { SetSegment(ActiveSegment, GetSegmentValue() + (e.Key == Key.Up ? 1 : -1)); e.Handled = true; return; }
        if (e.Key == Key.Escape) { _digits = ""; e.Handled = true; return; }
        var digit = Digit(e.Key); if (digit < 0) return; InputDigit(digit); e.Handled = true;
    }
    int GetSegmentValue() => ActiveSegment switch { XYTimeSegment.Hour => Time.Hour, XYTimeSegment.Minute => Time.Minute, _ => Time.Second };
    void MoveSegment(int direction) { var order = SegmentOrder(); var index = Array.IndexOf(order, ActiveSegment); ActivateSegment(order[(index + direction + order.Length) % order.Length]); _digits = ""; }
    void InputDigit(int digit) { _digits = _digits.Length >= 2 ? "" : _digits; _digits += digit; if (int.TryParse(_digits, out var value) && (ActiveSegment == XYTimeSegment.Hour ? value < 24 : value < 60)) SetSegment(ActiveSegment, value); if (_digits.Length == 2) { _digits = ""; MoveSegment(1); } }
    static int Digit(Key key) => key switch { >= Key.D0 and <= Key.D9 => key - Key.D0, >= Key.NumPad0 and <= Key.NumPad9 => key - Key.NumPad0, _ => -1 };
}
