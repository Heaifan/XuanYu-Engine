using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed partial class GroundAuthoringContractTests
{
    [Fact]
    public void No_terrain_uses_reference_plane()
    {
        var result = Pick(null, 0);
        Assert.True(result.IsValid);
        Assert.Equal(SurfaceBindingKind.ReferencePlane, result.SurfaceBinding.Kind);
        Assert.Equal(0, result.ResolvedElevation);
    }

    [Fact]
    public void Valid_terrain_hit_uses_terrain_and_elevation()
    {
        var result = Pick(new FakeSurface(12), 0);
        Assert.True(result.IsValid);
        Assert.Equal(SurfaceBindingKind.Terrain, result.SurfaceBinding.Kind);
        Assert.Equal(12, result.ResolvedElevation);
    }

    [Fact]
    public void Terrain_query_failure_is_invalid_without_plane_fallback() =>
        Assert.False(Pick(new FakeSurface(null), 0).IsValid);

    [Fact]
    public void Terrain_failure_does_not_create_region_vertex()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var camera = new CameraState(new(50, 50, 100), new(0, 0, -1),
            new(0, 1, 0), 45, 0.1, 1000, 1, ProjectionMode.Orthographic, 100);
        var projection = ViewProjectionState.Create(camera, new(0, 0, 100, 100, 100, 100, 1, 1));
        var state = new RegionDrawingState();
        state.Start(map.Layers.First(item => item.Kind == MapLayerKind.Region).LayerId,
            "R", MapRegionKind.Generic);
        Assert.False(MapSurfacePicker.TryPick(map, projection, 50, 50,
            new FakeSurface(null), out _));
        Assert.Empty(state.Draft!.Vertices);
    }

    [Fact]
    public void Region_keeps_xy_footprint_and_stable_binding()
    {
        var region = new MapRegion(MapRegionId.New(), MapLayerId.New(), "R",
            MapRegionKind.Generic, [new(1, 2), new(3, 2), new(2, 4)])
        { SurfaceBinding = SurfaceBinding.Terrain("dem-a") };
        Assert.True(region.Vertices.SequenceEqual([new(1, 2), new(3, 2), new(2, 4)]));
        Assert.Equal("dem-a", region.SurfaceBinding.Identity);
    }

    [Fact]
    public void Render_origin_does_not_change_region_world_xy()
    {
        var point = new MapPoint(42, -7);
        var first = MapCoordinateContract.MapToWorld(point, 100) - new Vector3d(10, 20, 30);
        var second = MapCoordinateContract.MapToWorld(point, 100) - new Vector3d(1000, 2000, 3000);
        Assert.Equal(point, MapCoordinateContract.WorldToMap(first + new Vector3d(10, 20, 30)));
        Assert.Equal(point, MapCoordinateContract.WorldToMap(second + new Vector3d(1000, 2000, 3000)));
    }

    static GroundPickResult Pick(IGroundSurface? surface, double plane) =>
        GroundPickResolver.Resolve(new WorldRay(new(10, 20, 100), new(0, 0, -1)), surface, plane);
}
