using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4LoadingIndicatorTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4LoadingIndicatorTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Loading_indicator_reuses_spinner_and_supports_detail_copy() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var indicator = new XYLoadingIndicator
        {
            Text = "正在初始化渲染视口",
            SecondaryText = "Vulkan 后端",
            Size = XyuiSpinnerSize.Compact,
            Variant = XyuiLoadingIndicatorVariant.Detail,
            IsActive = false
        };
        var window = XyuiBatchTestHost.Show(indicator);
        var spinner = Assert.Single(indicator.GetVisualDescendants().OfType<XYSpinner>());
        var labels = indicator.GetVisualDescendants().OfType<TextBlock>().ToArray();
        Assert.Equal("XYUI-4-4.14", indicator.CanonicalId);
        Assert.Same(indicator.Spinner, spinner); Assert.Equal(XyuiSpinnerSize.Compact, spinner.Size);
        Assert.Equal("正在初始化渲染视口", labels[0].Text); Assert.Equal("Vulkan 后端", labels[1].Text);
        Assert.False(spinner.IsActive); Assert.False(indicator.IsHitTestVisible);
        window.Close();
    });

    [Fact]
    public void Inline_indicator_hides_empty_secondary_copy() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var indicator = new XYLoadingIndicator { Variant = XyuiLoadingIndicatorVariant.Inline };
        var window = XyuiBatchTestHost.Show(indicator);
        var secondary = indicator.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-loading-indicator-secondary"));
        Assert.False(secondary.IsVisible); window.Close();
    });
}
