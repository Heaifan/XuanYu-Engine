using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYTimePicker
{
    void OnTimeKeyDown(KeyEventArgs e)
    {
        if (!IsEnabled) return;
        if (e.Key is Key.Left or Key.Right) { MoveSegment(e.Key == Key.Right ? 1 : -1); e.Handled = true; return; }
        if (e.Key is Key.Up or Key.Down) { CommitSegmentEdit(); SetSegment(ActiveSegment, GetSegmentValue() + (e.Key == Key.Up ? 1 : -1)); BeginSegmentEdit(ActiveSegment); e.Handled = true; return; }
        if (e.Key == Key.Escape) { CancelSegmentEdit(); e.Handled = true; return; }
        var digit = Digit(e.Key); if (digit < 0) return; InputDigit(digit); e.Handled = true;
    }
    int GetSegmentValue() => ActiveSegment switch { XYTimeSegment.Hour => Time.Hour, XYTimeSegment.Minute => Time.Minute, _ => Time.Second };
    void MoveSegment(int direction) { var order = SegmentOrder(); var index = Array.IndexOf(order, ActiveSegment); ActivateSegment(order[(index + direction + order.Length) % order.Length]); }
    void InputDigit(int digit) { if (!IsSegmentEditing) BeginSegmentEdit(ActiveSegment); EditBuffer = EditBuffer.Length >= 2 ? "" : EditBuffer; EditBuffer += digit; if (EditBuffer.Length != 2) return; if (int.TryParse(EditBuffer, out var value) && IsValidSegmentValue(value)) { CommitSegmentEdit(); MoveSegment(1); } else EditBuffer = ""; }
    static int Digit(Key key) => key switch { >= Key.D0 and <= Key.D9 => key - Key.D0, >= Key.NumPad0 and <= Key.NumPad9 => key - Key.NumPad0, _ => -1 };
}
