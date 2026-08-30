using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYDatePicker
{
    void OnDateKeyDown(KeyEventArgs e)
    {
        if (!IsEnabled) return;
        if (e.Key is Key.Left or Key.Right) { MoveSegment(e.Key == Key.Right ? 1 : -1); e.Handled = true; return; }
        if (e.Key is Key.Up or Key.Down) { AdjustSegment(e.Key == Key.Up ? 1 : -1); e.Handled = true; return; }
        if (e.Key == Key.Enter) { CommitSegmentEdit(); OpenDatePopup(ActiveSegment); e.Handled = true; return; }
        if (e.Key == Key.Escape) { if (IsDatePopupOpen) CancelDatePopup(); else { CancelSegmentEdit(); CloseCalendarForLifecycle(); } e.Handled = true; return; }
        var digit = Digit(e.Key); if (digit < 0) return; InputDigit(digit); e.Handled = true;
    }
    void MoveSegment(int direction) { var order = SegmentOrder().ToArray(); var index = Array.IndexOf(order, ActiveSegment); ActivateSegment(order[(index + direction + order.Length) % order.Length]); }
    void AdjustSegment(int amount) { CommitSegmentEdit(); SelectedDate = ActiveSegment switch { XYDateSegment.Year => SafeYear(amount), XYDateSegment.Month => SelectedDate.AddMonths(amount), _ => SelectedDate.AddDays(amount) }; BeginSegmentEdit(ActiveSegment); }
    DateOnly SafeYear(int amount) { try { return SelectedDate.AddYears(amount); } catch (ArgumentOutOfRangeException) { return amount > 0 ? new DateOnly(9999, 12, 31) : new DateOnly(1, 1, 1); } }
    void InputDigit(int digit) { if (!IsSegmentEditing) BeginSegmentEdit(ActiveSegment); EditBuffer = EditBuffer.Length >= SegmentWidth() ? "" : EditBuffer; EditBuffer += digit; if (EditBuffer.Length != SegmentWidth()) return; if (int.TryParse(EditBuffer, out var value) && TryBuild(value, out _)) { CommitSegmentEdit(); MoveSegment(1); } else EditBuffer = ""; }
    bool TryBuild(int value, out DateOnly date)
    {
        date = SelectedDate;
        try { date = ActiveSegment switch { XYDateSegment.Year => new DateOnly(value, Math.Min(SelectedDate.Month, 12), Math.Min(SelectedDate.Day, DateTime.DaysInMonth(value, Math.Min(SelectedDate.Month, 12)))), XYDateSegment.Month => new DateOnly(SelectedDate.Year, value, Math.Min(SelectedDate.Day, DateTime.DaysInMonth(SelectedDate.Year, value))), _ => new DateOnly(SelectedDate.Year, SelectedDate.Month, value) }; return value > 0; } catch (ArgumentOutOfRangeException) { return false; }
    }
    static int Digit(Key key) => key switch { >= Key.D0 and <= Key.D9 => key - Key.D0, >= Key.NumPad0 and <= Key.NumPad9 => key - Key.NumPad0, _ => -1 };
}
