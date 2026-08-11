using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public sealed record MapRegionDatasetFeature(
    MapRegionId RegionId,
    ImmutableArray<MapPoint> Points,
    string Name,
    MapRegionKind Kind);
