using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Viewport;

public sealed class ScaleBarTruthfulnessTests
{
    [Fact] public void ScaleBarFriendlyStepSeriesTest()
    {
        foreach (var meters in new[] { 1.0, 2, 5, 10, 20, 50, 100, 200, 500, 1000 })
        {
            var metric = ScaleIndicatorMetric.FromMetersPerDip(meters / 104.0);
            Assert.Equal(meters, metric.DistanceMeters);
        }
    }

    [Fact] public void PerspectiveScaleBarGroundIntersectionTest()
    {
        Assert.True(ScaleBarGroundProjection.TryCreate(Perspective(), Viewport(), 0, out var metric));
        Assert.True(metric.IsValid);
        Assert.InRange(metric.MetersPerDip, 0.01, 10.0);
    }

    [Fact] public void OrthographicScaleBarLinearScaleTest()
    {
        Assert.True(ScaleBarGroundProjection.TryCreate(Orthographic(100), Viewport(), 0, out var a));
        Assert.True(ScaleBarGroundProjection.TryCreate(Orthographic(200), Viewport(), 0, out var b));
        Assert.Equal(2.0, b.MetersPerDip / a.MetersPerDip, 6);
    }

    [Fact] public void DemBaselineGroundIntersectionTest() =>
        Assert.True(ScaleBarGroundProjection.TryCreate(Perspective(), Viewport(), 25, out var metric) &&
            metric.IsValid && metric.EndpointDistanceMeters > 0.0);

    [Fact] public void InvalidGroundIntersectionHidesScaleBarTest()
    {
        Assert.False(ScaleBarGroundProjection.TryCreate(Horizontal(), Viewport(), 0, out _));
        Assert.False(ScaleIndicatorMetric.FromMetersPerDip(double.NaN).IsVisible);
    }

    [Fact] public void UnitSwitchMetersToKilometersTest() =>
        Assert.Equal("1 km", ScaleIndicatorMetric.FromMetersPerDip(1000.0 / 104).Label);

    [Fact] public void ScaleBarPixelRangeTest()
    {
        foreach (var metersPerDip in new[] { 0.5, 1.0, 2.0, 10.0 })
            Assert.InRange(ScaleIndicatorMetric.FromMetersPerDip(metersPerDip).WidthDip, 80.0, 160.0);
    }

    [Fact] public void ScaleBarNoMisleadingFallbackTest() =>
        Assert.False(ScaleIndicatorMetric.FromMetersPerDip(0).IsVisible);

    static ViewportState Viewport() => new(0, 0, 800, 600, 800, 600, 1, 1);
    static RenderCameraProjection Perspective() =>
        new(new Vector3d(0, 0, 100), new Vector3d(0, 0, -1), Vector3d.UnitY, 60, 0.1, 1000, 1);
    static RenderCameraProjection Orthographic(double scale) =>
        new(new Vector3d(0, 0, 100), new Vector3d(0, 0, -1), Vector3d.UnitY, 60, 0.1, 1000, 1,
            ProjectionMode.Orthographic, scale);
    static RenderCameraProjection Horizontal() =>
        new(new Vector3d(0, 0, 100), Vector3d.UnitY, Vector3d.UnitZ, 60, 0.1, 1000, 1);
}
