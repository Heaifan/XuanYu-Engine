using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Viewport;

public sealed class GridScaleBarConvergenceTests
{
    [Fact]
    public void GridScaleBarScaleTrendConsistencyTest()
    {
        var scales = new[] { 0.5, 1.0, 2.0, 5.0, 10.0, 20.0, 50.0, 100.0 };
        var previousGrid = 0.0;
        var previousBar = 0.0;
        foreach (var metersPerDip in scales)
        {
            var grid = ReferenceGridScale.Compute(new ViewportMetricScale(
                metersPerDip, metersPerDip, 1.0));
            var bar = ScaleIndicatorMetric.FromMetersPerDip(metersPerDip);
            Assert.InRange(grid.FineWeight, 0.0, 1.0);
            Assert.InRange(grid.CoarseWeight, 0.0, 1.0);
            Assert.Equal(1.0, grid.FineWeight + grid.CoarseWeight, 6);
            Assert.True(grid.CoarseSpacing >= previousGrid);
            Assert.True(bar.DistanceMeters >= previousBar);
            Assert.InRange(bar.WidthDip, 70.0, 200.0);
            previousGrid = grid.CoarseSpacing;
            previousBar = bar.DistanceMeters;
        }
    }

    [Fact]
    public void GridAndScaleBarUseTheSameGroundScaleTrendTest()
    {
        var close = new ViewportMetricScale(2.0, 2.0, 1.0);
        var far = new ViewportMetricScale(20.0, 20.0, 1.0);
        var closeGrid = ReferenceGridScale.Compute(close);
        var farGrid = ReferenceGridScale.Compute(far);
        var closeBar = ScaleIndicatorMetric.FromMetersPerDip(close.MetersPerDip);
        var farBar = ScaleIndicatorMetric.FromMetersPerDip(far.MetersPerDip);
        Assert.True(farGrid.FineSpacing >= closeGrid.FineSpacing);
        Assert.True(farGrid.CoarseSpacing >= closeGrid.CoarseSpacing);
        Assert.True(farBar.DistanceMeters >= closeBar.DistanceMeters);
    }
}
