using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public sealed record MapRoadDatasetFeature(
    MapRoadId RoadId,
    ImmutableArray<MapPoint> Points,
    string Name,
    string Kind);
