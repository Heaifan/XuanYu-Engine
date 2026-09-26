using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4ProgressBarTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4ProgressBarTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void ProgressBar_has_safe_default_and_theme_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var bar = new XYProgressBar();
        var window = XyuiBatchTestHost.Show(bar);
        Assert.Equal("XYUI-4-4.16", bar.CanonicalId);
        Assert.Equal(0, bar.Minimum); Assert.Equal(100, bar.Maximum); Assert.Equal(0, bar.Value);
        Assert.Equal(0, bar.ProgressFraction); Assert.Equal(0, bar.Percentage);
        Assert.False(bar.IsIndeterminate); Assert.True(bar.ShowPercentage);
        Assert.Null(bar.StatusText); Assert.IsType<SolidColorBrush>(bar.Track);
        Assert.IsType<SolidColorBrush>(bar.Fill); Assert.False(bar.Focusable);
        window.Close();
    });

    [Theory]
    [InlineData(0, 0)] [InlineData(50, 0.5)] [InlineData(100, 1)]
    public void ProgressBar_reports_normalized_progress(double value, double fraction) => _fx.Run(() =>
    {
        var bar = new XYProgressBar { Minimum = 0, Maximum = 100, Value = value };
        Assert.Equal(fraction, bar.ProgressFraction, 3); Assert.Equal((int)value, bar.Percentage);
    });

    [Fact]
    public void ProgressBar_clamps_out_of_range_values_and_keeps_variant_contract() => _fx.Run(() =>
    {
        var bar = new XYProgressBar { Minimum = 10, Maximum = 20, Value = -5,
            Variant = XyuiProgressBarVariant.SegmentedStage, Size = XyuiProgressBarSize.Compact };
        Assert.Equal(10, bar.Value); Assert.Equal(0, bar.ProgressFraction); Assert.Equal(0, bar.Percentage);
        bar.Value = 80;
        Assert.Equal(20, bar.Value); Assert.Equal(1, bar.ProgressFraction); Assert.Equal(100, bar.Percentage);
        Assert.Equal(XyuiProgressBarVariant.SegmentedStage, bar.Variant);
        Assert.Equal(XyuiProgressBarSize.Compact, bar.Size);
    });

    [Fact]
    public void ProgressBar_supports_label_and_indeterminate_state_without_focus() => _fx.Run(() =>
    {
        var bar = new XYProgressBar { ShowPercentage = false, StatusText = "正在导入 DEM", IsIndeterminate = true };
        Assert.False(bar.ShowPercentage); Assert.Equal("正在导入 DEM", bar.StatusText);
        Assert.True(bar.IsIndeterminate); Assert.False(bar.IsHitTestVisible);
    });
}
