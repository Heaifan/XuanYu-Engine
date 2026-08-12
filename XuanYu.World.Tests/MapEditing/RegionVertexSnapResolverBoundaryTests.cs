using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionVertexSnapResolverBoundaryTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void Query_bounds_are_local_and_projection_failure_returns_raw()
    {
        var map = Map(); var projection = Projection(); RegionSpatialBounds captured = default;
        var raw = new MapPoint(2, 3); var state = new RegionVertexSnapState();
        var result = RegionVertexSnapResolver.Resolve(map.Regions[0].RegionId, raw, 400, 300, map, projection, state,
            bounds => { captured = bounds; return []; }, _ => null, RegionVertexSnapSettings.Default);
        Assert.False(result.IsSnapped); Assert.True(captured.MaxX > captured.MinX && captured.MaxY > captured.MinY);
    }

    [Fact]
    public void Query_failure_returns_raw_without_fallback()
    {
        var map = Map(); var state = new RegionVertexSnapState();
        var result = RegionVertexSnapResolver.Resolve(map.Regions[0].RegionId, new(1, 2), 400, 300, map, Projection(), state,
            _ => throw new InvalidOperationException(), _ => map.Regions[1], RegionVertexSnapSettings.Default);
        Assert.False(result.IsSnapped); Assert.Equal(new MapPoint(1, 2), result.ResolvedPoint);
    }

    static MapDefinition Map() => MapDefaultDefinition.CreateDefault() with
    {
        Regions = [Region(MapRegionId.New(), 0), Region(MapRegionId.New(), 100)]
    };

    static MapRegion Region(MapRegionId id, double x) => new(id, MapDefaultDefinition.CreateDefault().Layers[2].LayerId,
        "区域", MapRegionKind.Generic, [new(x, 0), new(x + 20, 0), new(x, 20)]);

    static ViewProjectionState Projection() => ViewProjectionState.Create(new CameraState(new Vector3d(0, 0, 1000),
        new Vector3d(0, 0, -1), Vector3d.UnitY, 60, 0.1, 5000, 1, ProjectionMode.Orthographic, 1200), Viewport);
}
