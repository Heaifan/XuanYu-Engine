using System.Collections.Immutable;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionVertexSnapResolverTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void No_candidates_returns_raw_and_does_not_change_state()
    {
        var map = Map(); var projection = Projection(); var state = new RegionVertexSnapState();
        var raw = new MapPoint(10, 10);
        var result = Resolve(map, projection, state, raw, 400, 300, []);
        Assert.Equal(raw, result.ResolvedPoint);
        Assert.False(result.IsSnapped);
        Assert.False(state.IsSnapped);
    }

    [Fact]
    public void Nearby_vertex_snaps_within_enter_radius()
    {
        var map = Map(); var target = map.Regions[1]; var projection = Projection();
        var screen = projection.ProjectWorldPoint(new(target.Vertices[0].X, target.Vertices[0].Y, 0));
        var result = Resolve(map, projection, new(), new(0, 0), screen.X + 4, screen.Y, [target.RegionId]);
        Assert.True(result.IsSnapped);
        Assert.Equal(target.RegionId, result.TargetRegionId);
        Assert.Equal(0, result.TargetVertexIndex);
    }

    [Fact]
    public void Candidate_outside_enter_radius_returns_raw()
    {
        var map = Map(); var target = map.Regions[1]; var projection = Projection();
        var screen = projection.ProjectWorldPoint(new(target.Vertices[0].X, target.Vertices[0].Y, 0));
        var result = Resolve(map, projection, new(), new(0, 0), screen.X + 9, screen.Y + 9, [target.RegionId]);
        Assert.False(result.IsSnapped);
    }

    [Fact]
    public void Source_region_is_excluded()
    {
        var map = Map(); var source = map.Regions[0]; var projection = Projection();
        var screen = projection.ProjectWorldPoint(new(source.Vertices[0].X, source.Vertices[0].Y, 0));
        var result = Resolve(map, projection, new(), source.Vertices[0], screen.X, screen.Y, [source.RegionId]);
        Assert.False(result.IsSnapped);
    }

    [Fact]
    public void Nearest_vertex_wins_over_later_candidate()
    {
        var map = Map(); var projection = Projection(); var first = map.Regions[1]; var second = map.Regions[2];
        var screen = projection.ProjectWorldPoint(new(200, 0, 0));
        var result = Resolve(map, projection, new(), new(0, 0), screen.X, screen.Y, [second.RegionId, first.RegionId]);
        Assert.Equal(second.RegionId, result.TargetRegionId);
    }

    [Fact]
    public void Equal_distance_uses_region_id_then_vertex_index()
    {
        var map = Map() with { Regions = [Map().Regions[0], Region(MapRegionId.New(), 100), Region(MapRegionId.New(), 100)] };
        var projection = Projection(); var a = map.Regions[1]; var b = map.Regions[2];
        var screen = projection.ProjectWorldPoint(new(100, 0, 0));
        var result = Resolve(map, projection, new(), new(0, 0), screen.X, screen.Y, [b.RegionId, a.RegionId]);
        Assert.Equal(new[] { a.RegionId, b.RegionId }.OrderBy(id => id.Value).First(), result.TargetRegionId);
        Assert.Equal(0, result.TargetVertexIndex);
    }

    static RegionVertexSnapResult Resolve(MapDefinition map, ViewProjectionState projection, RegionVertexSnapState state,
        MapPoint raw, double x, double y, ImmutableArray<MapRegionId> candidates) =>
        RegionVertexSnapResolver.Resolve(map.Regions[0].RegionId, raw, x, y, map, projection, state,
            _ => candidates, id => map.Regions.FirstOrDefault(region => region.RegionId == id), RegionVertexSnapSettings.Default);

    static MapDefinition Map() => MapDefaultDefinition.CreateDefault() with
    {
        Regions = [
            Region(MapRegionId.New(), 0), Region(MapRegionId.New(), 100), Region(MapRegionId.New(), 200)]
    };

    static MapRegion Region(MapRegionId id, double x) => new(id, MapDefaultDefinition.CreateDefault().Layers[2].LayerId,
        "区域", MapRegionKind.Generic, [new(x, 0), new(x + 20, 0), new(x, 20)]);

    static ViewProjectionState Projection() => ViewProjectionState.Create(new CameraState(new Vector3d(0, 0, 1000),
        new Vector3d(0, 0, -1), Vector3d.UnitY, 60, 0.1, 5000, 1, ProjectionMode.Orthographic, 1200), Viewport);
}
