using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

static class RegionSnapPipelineTestFixture
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    public static RegionEdgeSnapResult Resolve(MapDefinition map, MapRegion source, ScreenPoint pointer,
        ViewProjectionState projection, RegionSnapState state) => RegionSnapPipeline.Resolve(source.RegionId,
        new(pointer.X, pointer.Y), pointer, map, projection, state, _ => map.Regions.Select(r => r.RegionId).ToImmutableArray(),
        id => map.Regions.FirstOrDefault(region => region.RegionId == id), RegionEdgeSnapSettings.Default);

    public static (MapDefinition Map, MapRegion Source, MapRegion Target, ViewProjectionState Projection) Create()
    {
        var source = Region(MapRegionId.New(), [new(-200, -100), new(-150, -100), new(-200, -50)]);
        var target = Region(MapRegionId.New(), [new(0, 0), new(100, 0), new(100, 100), new(0, 100)]);
        return (Map([source, target]), source, target, Projection());
    }

    public static MapDefinition Map(ImmutableArray<MapRegion> regions) =>
        MapDefaultDefinition.CreateDefault() with { Regions = regions };
    public static MapRegion Region(MapRegionId id, ImmutableArray<MapPoint> points) =>
        new(id, MapDefaultDefinition.CreateDefault().Layers[2].LayerId, "区域", MapRegionKind.Generic, points);
    public static ViewProjectionState Projection() => ViewProjectionState.Create(new CameraState(new(0, 0, 1000),
        new Vector3d(0, 0, -1), Vector3d.UnitY, 60, 0.1, 5000, 1, ProjectionMode.Orthographic, 1200), Viewport);
    public static Vector3d World(MapPoint point) => new(point.X, point.Y, 0);
}
