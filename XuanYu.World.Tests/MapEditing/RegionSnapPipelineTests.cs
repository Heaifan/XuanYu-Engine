using XuanYu.Core.Gizmo;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSnapPipelineTests
{
    [Fact]
    public void Free_promotes_to_vertex_before_edge()
    {
        var (map, source, target, projection) = RegionSnapPipelineTestFixture.Create();
        var point = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(target.Vertices[0]));
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, point, projection, new());
        Assert.Equal(RegionSnapKind.Vertex, result.Kind);
    }

    [Fact]
    public void Free_promotes_to_edge_when_vertex_is_not_hit()
    {
        var (map, source, target, projection) = RegionSnapPipelineTestFixture.Create();
        var edge = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 5), projection, new());
        Assert.Equal(RegionSnapKind.Edge, result.Kind); Assert.Equal(target.RegionId, result.TargetRegionId);
    }

    [Fact]
    public void Source_region_is_never_an_edge_candidate()
    {
        var region = RegionSnapPipelineTestFixture.Region(MapRegionId.New(), [new(0, 0), new(100, 0), new(100, 100)]);
        var map = RegionSnapPipelineTestFixture.Map([region]); var projection = RegionSnapPipelineTestFixture.Projection();
        var edge = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        var result = RegionSnapPipelineTestFixture.Resolve(map, region, new(edge.X, edge.Y + 5), projection, new());
        Assert.Equal(RegionSnapKind.None, result.Kind);
    }

    [Fact]
    public void Local_query_is_the_only_candidate_source()
    {
        var (map, source, _, projection) = RegionSnapPipelineTestFixture.Create(); var called = false;
        var point = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        var result = RegionSnapPipeline.Resolve(source.RegionId, new(50, 4), new(point.X, point.Y + 5), map,
            projection, new RegionSnapState(), _ => { called = true; return []; },
            id => map.Regions.FirstOrDefault(region => region.RegionId == id), RegionEdgeSnapSettings.Default);
        Assert.True(called); Assert.False(result.IsSnapped);
    }

    [Fact]
    public void Local_query_extent_uses_release_radius()
    {
        var (map, source, _, projection) = RegionSnapPipelineTestFixture.Create(); RegionSpatialBounds captured = default;
        var point = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        RegionSnapPipeline.Resolve(source.RegionId, new(50, 4), new(point.X, point.Y + 5), map, projection,
            new RegionSnapState(), bounds => { captured = bounds; return []; }, _ => null, RegionEdgeSnapSettings.Default);
        Assert.True(captured.MaxX > captured.MinX && captured.MaxY > captured.MinY);
        RegionSnapQuery.TryBounds(point.X, point.Y, 8, map, projection, out var enterBounds);
        Assert.True(captured.MaxX - captured.MinX > enterBounds.MaxX - enterBounds.MinX);
    }

    [Fact]
    public void Query_failure_clears_stale_snap_target()
    {
        var (map, source, _, projection) = RegionSnapPipelineTestFixture.Create(); var state = new RegionSnapState();
        var edge = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 5), projection, state);
        var result = RegionSnapPipeline.Resolve(source.RegionId, new(50, 4), new(edge.X, edge.Y + 5), map, projection,
            state, _ => throw new InvalidOperationException(), _ => null, RegionEdgeSnapSettings.Default);
        Assert.Equal(RegionSnapKind.None, result.Kind); Assert.False(state.IsSnapped);
    }
}
