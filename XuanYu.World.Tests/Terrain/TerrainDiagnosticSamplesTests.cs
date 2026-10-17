using XuanYu.Editor.UI;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainDiagnosticSamplesTests
{
    [Fact]
    public void Replay_defaults_to_production_density_unless_override_is_requested()
    {
        Assert.Equal(513, TerrainDiagnosticSamples.Resolve(false, "2049"));
        Assert.Equal(513, TerrainDiagnosticSamples.Resolve(true, null));
        Assert.Equal(513, TerrainDiagnosticSamples.Resolve(true, "513"));
        Assert.Equal(1025, TerrainDiagnosticSamples.Resolve(true, "1025"));
        Assert.Equal(2049, TerrainDiagnosticSamples.Resolve(true, "2049"));
    }

    [Theory]
    [InlineData("2048")]
    [InlineData("bad")]
    public void Replay_rejects_unsupported_density(string requested)
    {
        Assert.Throws<InvalidOperationException>(() =>
            TerrainDiagnosticSamples.Resolve(true, requested));
    }

    [Fact]
    public void Render_surface_clearance_uses_triangle_height_and_vertical_exaggeration()
    {
        var field = new TerrainHeightfield(2, 2, [10d, 20d, 30d, 40d]);
        var resource = new TerrainRenderResource("tile", 1, field, 1,
            new Vector3d(0, 0, 0));
        var surface = new TerrainRenderGroundSurface([resource]);

        var result = surface.MeasureVerticalClearance(0.25, 0.25, 40, 2);

        Assert.True(result.IsValid);
        Assert.Equal("Valid", result.QueryStatus);
        Assert.Equal(35, result.RenderSurfaceZ);
        Assert.Equal(5, result.VerticalClearanceMeters);
    }

    [Fact]
    public void Render_surface_clearance_reports_invalid_query_without_a_distance()
    {
        var field = new TerrainHeightfield(2, 2, [10d, 20d, 30d, 40d]);
        var surface = new TerrainRenderGroundSurface([
            new TerrainRenderResource("tile", 1, field, 1, new Vector3d(0, 0, 0))]);

        var result = surface.MeasureVerticalClearance(-1, 0, 40, 1);

        Assert.False(result.IsValid);
        Assert.Equal("OutOfBounds", result.QueryStatus);
        Assert.Null(result.RenderSurfaceZ);
        Assert.Null(result.VerticalClearanceMeters);
    }
}
