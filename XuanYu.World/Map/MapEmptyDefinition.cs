using System.Collections.Immutable;

namespace XuanYu.World.Map;

public static class MapEmptyDefinition
{
    public static MapDefinition Create() => new(
        default,
        "",
        new MapSize(0, 0),
        MapCoordinateSystem.ZUpMeter,
        MapSurfaceDefinition.DefaultFlat,
        ImmutableArray<MapLayer>.Empty,
        ImmutableArray<MapRegion>.Empty,
        ImmutableArray<MapRoad>.Empty,
        ImmutableArray<MapMarker>.Empty);
}
