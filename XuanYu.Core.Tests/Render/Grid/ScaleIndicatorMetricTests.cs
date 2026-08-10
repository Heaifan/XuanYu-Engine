using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ScaleIndicatorMetricTests
{
    [Theory]
    [InlineData(0.625, 65.0, "65 m")]
    [InlineData(1.0, 104.0, "104 m")]
    [InlineData(0.1, 10.4, "10.4 m")]
    [InlineData(1000.0, 104000.0, "104 km")]
    [InlineData(0.4, 41.6, "41.6 m")]
    [InlineData(0.01, 1.04, "1.04 m")]
    public void Chooses_pretty_distance_in_target_width(
        double metersPerDip, double distance, string label)
    {
        var metric = ScaleIndicatorMetric.FromMetersPerDip(metersPerDip);
        Assert.Equal(distance, metric.DistanceMeters, 6);
        Assert.Equal(ScaleIndicatorMetric.FixedBarWidthDip, metric.WidthDip, 6);
        Assert.Equal(label, metric.Label);
    }
}
