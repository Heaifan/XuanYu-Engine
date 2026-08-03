using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed partial class MapRegionTests
{
    static readonly MapSize Map10km = new(10000.0, 10000.0);

    static ImmutableArray<MapPoint> Square() => ImmutableArray.Create(
        new MapPoint(-100, -100), new MapPoint(100, -100),
        new MapPoint(100, 100), new MapPoint(-100, 100));

    static MapRegion Region(MapLayerId layerId, ImmutableArray<MapPoint>? vertices = null, bool closed = true) =>
        new(MapRegionId.New(), layerId, "部署区A", MapRegionKind.Deployment,
            vertices ?? Square(), closed);

    static ImmutableArray<MapLayer> Layers() => MapDefaultLayers.CreateDefault();
}
