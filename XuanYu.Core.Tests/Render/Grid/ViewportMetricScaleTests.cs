using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ViewportMetricScaleTests
{
    [Theory]
    [InlineData(1.0), InlineData(1.25), InlineData(1.5), InlineData(2.0)]
    public void Perspective_metric_is_dip_stable_across_dpi(double dpi)
    {
        var metric = CreateMetric(ProjectionMode.Perspective, 0.0, dpi);
        var baseline = CreateMetric(ProjectionMode.Perspective, 0.0, 1.0);
        Assert.Equal(baseline.MetersPerDip, metric.MetersPerDip, 6);
        Assert.Equal(metric.MetersPerDip / dpi, metric.MetersPerPhysicalPixel, 6);
    }

    [Theory]
    [InlineData(1.0), InlineData(1.25), InlineData(1.5), InlineData(2.0)]
    public void Orthographic_metric_is_dip_stable_across_dpi(double dpi)
    {
        var metric = CreateMetric(ProjectionMode.Orthographic, 100.0, dpi);
        Assert.Equal(100.0 / 600.0, metric.MetersPerDip, 6);
        Assert.Equal(metric.MetersPerDip / dpi, metric.MetersPerPhysicalPixel, 6);
    }

    static ViewportMetricScale CreateMetric(ProjectionMode mode, double scale, double dpi)
    {
        var camera = new RenderCameraProjection(
            new Vector3d(0, 0, 100), new Vector3d(0, 0, -1),
            new Vector3d(0, 1, 0), 60, 0.1, 1000, 1, mode, scale);
        var viewport = new ViewportState(0, 0, 800, 600,
            (int)(800 * dpi), (int)(600 * dpi), dpi, 1);
        Assert.True(ViewportMetricScale.TryCreate(camera, viewport, 0.0, out var metric));
        return metric;
    }
}
