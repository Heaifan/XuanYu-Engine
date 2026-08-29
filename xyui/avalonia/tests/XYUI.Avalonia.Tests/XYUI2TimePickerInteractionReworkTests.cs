using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2TimePickerInteractionReworkTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2TimePickerInteractionReworkTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void TimePicker_click_segments_enters_edit_and_replaces_values() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYTimePicker { Width = 220, Time = new TimeOnly(14, 30, 25) }; var window = XyuiBatchTestHost.Show(picker);
        Click(window, picker.SegmentButtons[XYTimeSegment.Hour]); Assert.True(picker.IsSegmentEditing); Type(picker, Key.D0, Key.D9); Assert.Equal(9, picker.Time.Hour);
        Click(window, picker.SegmentButtons[XYTimeSegment.Minute]); Type(picker, Key.D4, Key.D5); Assert.Equal(45, picker.Time.Minute);
        Click(window, picker.SegmentButtons[XYTimeSegment.Second]); Type(picker, Key.D0, Key.D8); Assert.Equal(8, picker.Time.Second); window.Close();
    });

    [Fact]
    public void TimePicker_click_without_drag_does_not_scrub() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYTimePicker { Width = 220 }; var window = XyuiBatchTestHost.Show(picker); Click(window, picker.SegmentButtons[XYTimeSegment.Minute]);
        Assert.False(picker.IsScrubArmed); Assert.False(picker.IsScrubbing); Assert.True(picker.IsSegmentEditing); window.Close();
    });

    [Fact]
    public void TimePicker_scrub_has_threshold_direction_and_release_commit() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYTimePicker { Width = 220, Time = new TimeOnly(10, 30, 10) }; var window = XyuiBatchTestHost.Show(picker); var minute = picker.SegmentButtons[XYTimeSegment.Minute]; var start = minute.TranslatePoint(new Point(8, 16), window)!.Value;
        window.MouseMove(start); window.MouseDown(start, MouseButton.Left); Assert.True(picker.IsScrubArmed); window.MouseMove(new Point(start.X + 2, start.Y)); Dispatcher.UIThread.RunJobs(); Assert.False(picker.IsScrubbing); window.MouseMove(new Point(start.X + 20, start.Y)); Dispatcher.UIThread.RunJobs(); Assert.True(picker.IsScrubbing); Assert.Equal(35, picker.Time.Minute); window.MouseUp(new Point(start.X + 20, start.Y), MouseButton.Left); Assert.Equal(35, picker.Time.Minute); Assert.False(picker.IsScrubbing); window.Close();
    });

    [Fact]
    public void TimePicker_capture_loss_detach_and_disabled_cancel_scrub() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYTimePicker { Width = 220, Time = new TimeOnly(10, 30, 10) }; var window = XyuiBatchTestHost.Show(picker); var minute = picker.SegmentButtons[XYTimeSegment.Minute]; var start = minute.TranslatePoint(new Point(8, 16), window)!.Value;
        window.MouseMove(start); window.MouseDown(start, MouseButton.Left); window.MouseMove(new Point(start.X + 20, start.Y)); Dispatcher.UIThread.RunJobs(); Assert.True(picker.IsScrubbing); picker.CancelScrub(); Assert.Equal(30, picker.Time.Minute); Assert.False(picker.IsScrubArmed);
        window.MouseDown(start, MouseButton.Left); window.MouseMove(new Point(start.X - 20, start.Y)); Dispatcher.UIThread.RunJobs(); Assert.True(picker.IsScrubbing); picker.IsEnabled = false; Assert.False(picker.IsScrubbing); Assert.Equal(30, picker.Time.Minute); window.Content = null; window.Close();
    });

    static void Click(Window window, Button button) { var point = button.TranslatePoint(new Point(8, 16), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); }
    static void Type(XYTimePicker picker, params Key[] keys) { foreach (var key in keys) { picker.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key }); Dispatcher.UIThread.RunJobs(); } }
}
