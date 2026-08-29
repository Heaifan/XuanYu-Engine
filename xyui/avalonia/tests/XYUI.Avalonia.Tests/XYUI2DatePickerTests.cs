using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2DatePickerTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2DatePickerTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void DatePicker_has_one_root_and_three_clickable_segments() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2026, 8, 12) }; var window = XyuiBatchTestHost.Show(picker);
        Assert.Equal(32, picker.Bounds.Height); Assert.Equal(3, picker.GetVisualDescendants().OfType<Button>().Count(x => x.Name is "PART_Year" or "PART_Month" or "PART_Day")); Assert.DoesNotContain(picker.GetVisualDescendants(), x => x is TextBox); window.Close();
    });

    [Fact]
    public void DatePicker_quick_steps_calendar_selection_and_lifecycle_work() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2028, 2, 28) }; var window = XyuiBatchTestHost.Show(picker); picker.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_Next").RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(new DateOnly(2028, 2, 29), picker.SelectedDate);
        picker.OpenCalendar(); Assert.True(picker.IsCalendarOpen); var calendar = Assert.IsAssignableFrom<Control>(picker.CalendarContentPart!.Content); var leap = calendar.GetVisualDescendants().OfType<Button>().Single(x => x.Content?.ToString() == "29"); leap.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(new DateOnly(2028, 2, 29), picker.SelectedDate); Assert.False(picker.IsCalendarOpen); picker.OpenCalendar(); window.Content = null; Assert.False(picker.IsCalendarOpen); window.Close();
    });

    [Fact]
    public void DatePicker_keyboard_changes_only_active_segment_and_honors_bounds() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYDatePicker { SelectedDate = new DateOnly(2026, 8, 12), MinDate = new DateOnly(2026, 1, 1), MaxDate = new DateOnly(2026, 12, 31) }; var window = XyuiBatchTestHost.Show(picker); picker.ActivateSegment(XYDateSegment.Month); Raise(picker, Key.Up); Assert.Equal(new DateOnly(2026, 9, 12), picker.SelectedDate); Raise(picker, Key.Down); Assert.Equal(new DateOnly(2026, 8, 12), picker.SelectedDate); picker.SelectedDate = new DateOnly(2026, 12, 31); Raise(picker, Key.Up); Assert.Equal(new DateOnly(2026, 12, 31), picker.SelectedDate); window.Close();
    });

    [Fact]
    public void DatePicker_gallery_has_required_real_dates_and_disabled_sample() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-17"); var pickers = preview.GetVisualDescendants().OfType<XYDatePicker>().ToArray(); Assert.True(pickers.Length >= 9); Assert.Contains(pickers, x => x.SelectedDate == new DateOnly(2028, 2, 29)); Assert.Contains(pickers, x => !x.IsEnabled); preview.ApplyStyling();
    });

    static void Raise(XYDatePicker picker, Key key) { picker.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key }); Dispatcher.UIThread.RunJobs(); }
}
