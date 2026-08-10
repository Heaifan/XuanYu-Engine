using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class ReferenceGridFrameStateTests
{
    [Fact]
    public void Step_is_global_and_hysteretic()
    {
        var initial = default(ReferenceGridFrameState);
        var atOneMeter = new ViewportMetricScale(1.0, 1.0, 1.0);
        var state = ReferenceGridFrameState.Create(atOneMeter, 12_431, -12_431, 20, initial);
        Assert.Equal(100.0, state.StepMeters);
        Assert.Equal(12_400.0, state.AnchorX);
        Assert.Equal(-12_400.0, state.AnchorY);
        var stable = ReferenceGridFrameState.Create(new(1.01, 0.99, 1.0), 12_448, -12_448, 20, state);
        Assert.Equal(state.StepMeters, stable.StepMeters);
        Assert.Equal(state.AnchorX, stable.AnchorX);
        Assert.Equal(state.AnchorY, stable.AnchorY);
    }

    [Theory]
    [InlineData(20.0, 1000.0)]
    [InlineData(0.5, 100.0)]
    [InlineData(0.05, 100.0)]
    [InlineData(0.005, 100.0)]
    public void Step_changes_only_at_global_cell_dip_bounds(double metersPerDip, double expectedStep)
    {
        var metric = new ViewportMetricScale(metersPerDip, metersPerDip, 1.0);
        var state = ReferenceGridFrameState.Create(metric, 0, 0, 0, default);
        Assert.Equal(expectedStep, state.StepMeters);
    }

    [Fact]
    public void Grid_geometry_is_fixed_at_513_lines_per_axis()
    {
        Assert.Equal(513, RenderDrawPlan.ReferenceGridLineCountPerAxis);
        Assert.Equal(2052, RenderDrawPlan.ReferenceGridLineVertexCount);
        Assert.Equal(3, RenderDrawPlan.FullscreenTriangleVertexCount);
    }
}
