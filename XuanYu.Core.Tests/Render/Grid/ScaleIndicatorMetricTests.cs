using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ScaleIndicatorMetricTests
{
    [Theory]
    [InlineData(1.0, 104.0, 100.0, "100 m")]
    [InlineData(1.215, 126.36, 120.0, "120 m")]
    [InlineData(5.9676923, 620.64, 620.0, "620 m")]
    [InlineData(6.6153846, 688.0, 680.0, "680 m")]
    [InlineData(18.6826923, 1943.0, 1900.0, "1900 m")]
    [InlineData(123.076923, 12800.0, 12000.0, "12 km")]
    [InlineData(1211.53846, 126000.0, 120000.0, "120 km")]
    public void Snaps_real_distance_down_to_two_significant_digits(
        double metersPerDip, double rawDistance, double distance, string label)
    {
        Assert.Equal(rawDistance, metersPerDip * ScaleIndicatorMetric.FixedBarWidthDip, 3);
        var metric = ScaleIndicatorMetric.FromMetersPerDip(metersPerDip);
        Assert.Equal(distance, metric.DistanceMeters, 6);
        Assert.Equal(distance / metersPerDip, metric.WidthDip, 5);
        Assert.Equal(label, metric.Label);
    }

    [Fact]
    public void Hides_below_100m_without_affecting_camera_policy()
    {
        var metric = ScaleIndicatorMetric.FromMetersPerDip(99.9 / ScaleIndicatorMetric.FixedBarWidthDip);
        Assert.Equal(0.0, metric.DistanceMeters);
        Assert.Equal(0.0, metric.WidthDip);
        Assert.Equal("", metric.Label);
    }
}
