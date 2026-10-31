using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4SpinnerTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4SpinnerTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Spinner_exposes_open_arc_contract_and_canonical_sizes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var spinner = new XYSpinner { Size = XyuiSpinnerSize.Compact, IsActive = false };
        var window = XyuiBatchTestHost.Show(spinner);
        Assert.Equal("XYUI-4-4.15", spinner.CanonicalId);
        Assert.Equal(14, spinner.DesiredSize.Width); Assert.Equal(14, spinner.DesiredSize.Height);
        Assert.Equal(2, XYSpinner.StrokeFor(XyuiSpinnerSize.Compact));
        Assert.False(spinner.Focusable); Assert.False(spinner.IsHitTestVisible);
        Assert.IsType<SolidColorBrush>(spinner.Track); Assert.IsType<SolidColorBrush>(spinner.Arc);
        window.Close();
    });

    [Fact]
    public void Reduced_motion_keeps_the_arc_static() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var spinner = new XYSpinner { IsReducedMotion = true };
        var window = XyuiBatchTestHost.Show(spinner);
        var angle = spinner.CurrentAngleDegrees;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(angle, spinner.CurrentAngleDegrees);
        window.Close();
    });
}
