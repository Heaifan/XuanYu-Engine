using System.Collections.Immutable;

namespace XuanYu.World.Map;

public sealed record MapRoad(
    MapRoadId RoadId,
    MapLayerId LayerId,
    string DisplayName,
    string Kind,
    ImmutableArray<MapPoint> Points,
    bool IsVisible = true,
    bool IsLocked = false);
