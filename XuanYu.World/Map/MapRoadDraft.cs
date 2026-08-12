using System.Collections.Immutable;

namespace XuanYu.World.Map;

public sealed record MapRoadDraft(
    MapLayerId LayerId,
    string DisplayName,
    string Kind,
    ImmutableArray<MapPoint> Points)
{
    public bool CanComplete => Points.Length >= 2;
    public MapRoad Complete(MapRoadId id) => new(id, LayerId, DisplayName, Kind, Points);
}
