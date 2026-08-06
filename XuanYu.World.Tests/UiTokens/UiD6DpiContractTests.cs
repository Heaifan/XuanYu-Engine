using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD6DpiContractTests
{
    [Theory]
    [InlineData(1.0)]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(1.75)]
    [InlineData(2.0)]
    public void Contract_declares_supported_desktop_scales(double scale)
    {
        Assert.Contains(scale, UiDpiContract.SupportedScales);
        Assert.True(UiDpiContract.IsSupported(scale));
    }

    [Theory]
    [InlineData(12, 1.25, 15)]
    [InlineData(320, 1.5, 480)]
    [InlineData(360, 2.0, 720)]
    public void Physical_conversion_keeps_dip_as_source_of_truth(
        double dip, double scale, double expected)
    {
        Assert.Equal(expected, UiDpiContract.Physical(dip, scale), 3);
    }

    [Fact]
    public void Ergonomic_thresholds_remain_in_dip_not_physical_pixels()
    {
        Assert.True(UiDpiContract.MinWindowWidthDip >= 1280);
        Assert.True(UiDpiContract.RecommendedWindowWidthDip >= 1440);
        Assert.True(UiDpiContract.InspectorNarrowThresholdDip >= 320);
        Assert.True(UiDpiContract.MapFormWideThresholdDip >= 360);
        Assert.True(UiDpiContract.UsesDipThresholds);
    }
}
