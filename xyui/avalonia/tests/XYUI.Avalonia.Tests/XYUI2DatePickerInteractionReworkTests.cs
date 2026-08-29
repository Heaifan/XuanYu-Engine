using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2DatePickerInteractionReworkTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2DatePickerInteractionReworkTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void DatePicker_single_calendar_click_keeps_popup_open() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { Width = 300 }; var window = XyuiBatchTestHost.Show(picker); var button = picker.CalendarButtonPart!;
        var point = button.TranslatePoint(new Point(8, 16), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs();
        Assert.True(picker.IsCalendarOpen); Assert.True(picker.PopupPart!.IsOpen); window.Close();
    });

    [Fact]
    public void DatePicker_popup_selection_escape_and_light_dismiss_sync_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2028, 2, 28) }; var window = XyuiBatchTestHost.Show(picker);
        picker.OpenCalendar(); Assert.True(picker.IsCalendarOpen); var calendar = Assert.IsAssignableFrom<Control>(picker.CalendarContentPart!.Content); calendar.GetVisualDescendants().OfType<Button>().Single(x => x.Content?.ToString() == "29").RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(new DateOnly(2028, 2, 29), picker.SelectedDate); Assert.False(picker.IsCalendarOpen);
        picker.OpenCalendar(); Raise(picker, Key.Escape); Assert.False(picker.IsCalendarOpen); picker.OpenCalendar(); window.MouseDown(new Point(2, 2), MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.False(picker.IsCalendarOpen); picker.OpenCalendar(); picker.PopupPart!.IsOpen = false; Dispatcher.UIThread.RunJobs(); Assert.False(picker.IsCalendarOpen); window.Close();
    });

    [Fact]
    public void DatePicker_segments_enter_edit_and_replace_values() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { SelectedDate = new DateOnly(2026, 8, 12) }; var window = XyuiBatchTestHost.Show(picker);
        Click(window, picker.SegmentButtons[XYDateSegment.Year]); Assert.True(picker.IsSegmentEditing); Type(picker, Key.D2, Key.D0, Key.D2, Key.D7); Assert.Equal(2027, picker.SelectedDate.Year);
        Click(window, picker.SegmentButtons[XYDateSegment.Month]); Type(picker, Key.D1, Key.D1); Assert.Equal(11, picker.SelectedDate.Month); Click(window, picker.SegmentButtons[XYDateSegment.Day]); Type(picker, Key.D2, Key.D5); Assert.Equal(25, picker.SelectedDate.Day); window.Close();
    });

    [Fact]
    public void DatePicker_segment_commit_cancel_and_quick_steps_are_predictable() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { SelectedDate = new DateOnly(2026, 1, 31) }; var window = XyuiBatchTestHost.Show(picker);
        Click(window, picker.SegmentButtons[XYDateSegment.Month]); Type(picker, Key.D0); Click(window, picker.SegmentButtons[XYDateSegment.Day]); Assert.Equal(new DateOnly(2026, 1, 31), picker.SelectedDate); Type(picker, Key.D2, Key.D8); Assert.Equal(28, picker.SelectedDate.Day); Raise(picker, Key.Escape); Assert.False(picker.IsSegmentEditing);
        picker.PreviousPart!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(new DateOnly(2026, 1, 27), picker.SelectedDate); picker.NextPart!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(new DateOnly(2026, 1, 28), picker.SelectedDate); window.Close();
    });

    static void Click(Window window, Button button) { var point = button.TranslatePoint(new Point(8, 16), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); }
    static void Type(XYDatePicker picker, params Key[] keys) { foreach (var key in keys) Raise(picker, key); }
    static void Raise(XYDatePicker picker, Key key) { picker.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key }); Dispatcher.UIThread.RunJobs(); }
}
