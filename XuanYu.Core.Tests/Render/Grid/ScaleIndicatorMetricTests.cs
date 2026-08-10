using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ScaleIndicatorMetricTests
{
    [Theory]
    [InlineData(130.0, 100.0, "100 m")]
    [InlineData(190.0, 100.0, "100 m")]
    [InlineData(230.0, 200.0, "200 m")]
    [InlineData(480.0, 200.0, "200 m")]
    [InlineData(620.0, 500.0, "500 m")]
    [InlineData(950.0, 500.0, "500 m")]
    [InlineData(1300.0, 1000.0, "1 km")]
    [InlineData(2700.0, 2000.0, "2 km")]
    [InlineData(6200.0, 5000.0, "5 km")]
    [InlineData(10800.0, 10000.0, "10 km")]
    public void Chooses_largest_nice_distance_that_fits(double rawDistance, double distance, string label)
    {
        var metersPerDip = rawDistance / ScaleIndicatorMetric.FixedBarWidthDip;
        var metric = ScaleIndicatorMetric.FromMetersPerDip(metersPerDip);
        Assert.Equal(distance, metric.DistanceMeters, 6);
        Assert.Equal(distance / metersPerDip, metric.WidthDip, 5);
        Assert.Equal(label, metric.Label);
    }

    [Fact]
    public void Holds_previous_step_until_five_percent_below_boundary()
    {
        var metersPerDip = 499.0 / ScaleIndicatorMetric.FixedBarWidthDip;
        var metric = ScaleIndicatorMetric.FromMetersPerDip(metersPerDip, 500.0);
        Assert.Equal(500.0, metric.DistanceMeters, 6);
        Assert.Equal("500 m", metric.Label);
        var lower = ScaleIndicatorMetric.FromMetersPerDip(
            474.0 / ScaleIndicatorMetric.FixedBarWidthDip, 500.0);
        Assert.Equal(200.0, lower.DistanceMeters, 6);
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
