using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2TimePickerTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2TimePickerTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void TimePicker_has_hhmm_and_hhmmss_variants_without_textboxes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var withSeconds = new XYTimePicker { Width = 220, Time = new TimeOnly(14, 30, 25), ShowSeconds = true }; var withoutSeconds = new XYTimePicker { Width = 220, Time = new TimeOnly(9, 5), ShowSeconds = false }; var window = XyuiBatchTestHost.Show(new StackPanel { Children = { withSeconds, withoutSeconds } });
        Assert.Equal(32, withSeconds.Bounds.Height); Assert.Equal(3, withSeconds.GetVisualDescendants().Count(x => x is Button b && b.Name is "PART_Hour" or "PART_Minute" or "PART_Second")); Assert.Equal(2, withoutSeconds.GetVisualDescendants().Count(x => x is Button b && (b.Name is "PART_Hour" or "PART_Minute" or "PART_Second") && b.IsVisible)); Assert.DoesNotContain(withSeconds.GetVisualDescendants(), x => x is TextBox); window.Close();
    });

    [Fact]
    public void TimePicker_keyboard_wraps_only_current_segment() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYTimePicker { Time = new TimeOnly(23, 59, 10) }; var window = XyuiBatchTestHost.Show(picker); picker.ActivateSegment(XYTimeSegment.Minute); Raise(picker, Key.Up); Assert.Equal(new TimeOnly(23, 0, 10), picker.Time); Raise(picker, Key.Down); Assert.Equal(new TimeOnly(23, 59, 10), picker.Time); picker.ActivateSegment(XYTimeSegment.Hour); Raise(picker, Key.Up); Assert.Equal(new TimeOnly(0, 59, 10), picker.Time); window.Close();
    });

    [Fact]
    public void TimePicker_scrub_changes_value_and_clears_indicator_on_release() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var picker = new XYTimePicker { Width = 220, Time = new TimeOnly(10, 30, 10) };
        var window = XyuiBatchTestHost.Show(picker);
        var minute = picker.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_Minute");
        Assert.True(minute.Bounds.Width > 0, minute.Bounds.ToString());
        var start = minute.TranslatePoint(new Point(8, 16), window)!.Value;
        Assert.NotNull(window.InputHitTest(start));
        XyuiBatchTestHost.Hover(window, minute);
        Assert.Contains(":pointerover", minute.Classes);
        window.MouseMove(start); window.MouseDown(start, MouseButton.Left);
        Assert.True(picker.IsScrubArmed);
        window.MouseMove(new Point(start.X + 20, start.Y)); Dispatcher.UIThread.RunJobs();
        Assert.True(picker.IsScrubbing);
        Assert.Equal(35, picker.Time.Minute);
        window.MouseUp(new Point(start.X + 20, start.Y), MouseButton.Left);
        Assert.False(picker.IsScrubbing); Assert.False(picker.ScrubIndicatorPart!.IsVisible); window.Close();
    });

    [Fact]
    public void TimePicker_gallery_has_real_variants_and_disabled_sample() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-18"); var pickers = preview.GetVisualDescendants().OfType<XYTimePicker>().ToArray(); Assert.True(pickers.Length >= 6); Assert.Contains(pickers, x => !x.ShowSeconds); Assert.Contains(pickers, x => !x.IsEnabled); preview.ApplyStyling();
    });

    static void Raise(XYTimePicker picker, Key key) { picker.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key }); Dispatcher.UIThread.RunJobs(); }
}
