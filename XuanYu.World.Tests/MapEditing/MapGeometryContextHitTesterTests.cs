using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapGeometryContextHitTesterTests
{
    [Fact]
    public void Vertex_wins_over_edge_and_face()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var layer = map.ActiveLayer();
        var region = new MapRegion(MapRegionId.New(), layer, "区域", MapRegionKind.Generic,
            [new(-100, -100), new(100, -100), new(100, 100), new(-100, 100)]);
        var projection = Projection();
        map = map with { Regions = [region] };
        var point = projection.ProjectWorldPoint(new Vector3d(-100, -100, 0));

        Assert.True(MapGeometryContextHitTester.TryHit(map, projection, point.X, point.Y, 0, out var hit));
        Assert.Equal(MapGeometryContextKind.Vertex, hit.Kind);
        Assert.Equal(0, hit.VertexIndex);
    }

    [Fact]
    public void Road_segment_is_an_edge_hit_and_empty_space_is_empty()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var road = new MapRoad(MapRoadId.New(), map.ActiveLayer(), "道路", "generic", [new(-100, 0), new(100, 0)]);
        map = map with { Roads = [road] };
        var projection = Projection();
        var edge = projection.ProjectWorldPoint(new Vector3d(0, 0, 0));
        Assert.True(MapGeometryContextHitTester.TryHit(map, projection, edge.X, edge.Y, 0, out var hit));
        Assert.Equal(MapGeometryContextKind.Edge, hit.Kind);
        Assert.Equal(0, hit.SegmentIndex);
        Assert.False(MapGeometryContextHitTester.TryHit(map, projection, 20, 20, 0, out _));
    }

    static ViewProjectionState Projection() => ViewProjectionState.Create(
        new CameraState(new Vector3d(0, 0, 1000), new Vector3d(0, 0, -1), Vector3d.UnitY,
            60, 0.1, 5000, 1, ProjectionMode.Orthographic, 1200),
        new ViewportState(0, 0, 800, 600, 800, 600, 1, 1));
}
