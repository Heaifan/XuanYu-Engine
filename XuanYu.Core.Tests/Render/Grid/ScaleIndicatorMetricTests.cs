using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ScaleIndicatorMetricTests
{
    [Theory]
    [InlineData(0.625, 100.0, 160.0, "100 m")]
    [InlineData(1.0, 100.0, 100.0, "100 m")]
    [InlineData(0.1, 10.0, 100.0, "10 m")]
    [InlineData(1000.0, 100000.0, 100.0, "100 km")]
    [InlineData(0.4, 50.0, 125.0, "50 m")]
    [InlineData(0.01, 1.0, 100.0, "1 m")]
    public void Chooses_pretty_distance_in_target_width(
        double metersPerDip, double distance, double width, string label)
    {
        var metric = ScaleIndicatorMetric.FromMetersPerDip(metersPerDip);
        Assert.Equal(distance, metric.DistanceMeters, 6);
        Assert.Equal(width, metric.WidthDip, 6);
        Assert.Equal(label, metric.Label);
    }
}
