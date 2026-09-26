using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2ColorPickerEditLifecycleTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2ColorPickerEditLifecycleTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Color_changes_do_not_commit_until_picker_closes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var picker = new XYColorPicker { Mode = XYColorPickerMode.RGB };
        var window = XyuiBatchTestHost.Show(picker);
        var started = 0; var committed = 0; var canceled = 0;
        picker.EditStarted += (_, _) => started++;
        picker.EditCommitted += (_, _) => committed++;
        picker.EditCanceled += (_, _) => canceled++;

        picker.IsOpen = true;
        picker.Color = Color.FromRgb(200, 80, 40);
        picker.Color = Color.FromRgb(210, 90, 50);
        Assert.Equal(1, started);
        Assert.Equal(0, committed);

        picker.IsOpen = false;
        Assert.Equal(1, committed);
        Assert.Equal(0, canceled);

        picker.IsOpen = true;
        picker.Color = Color.FromRgb(20, 120, 80);
        picker.CancelEditLifecycle();
        Assert.Equal(1, canceled);
        Assert.Equal(1, committed);
        window.Close();
    });
}
