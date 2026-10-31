using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

internal readonly record struct RegionSpatialQueryResult(
    ImmutableArray<MapRegionId> RegionIds,
    RegionSpatialQueryStats Stats);

internal readonly record struct RegionSpatialQueryStats(
    int VisitedNodeCount,
    int TestedLeafCount,
    int MatchedCount,
    int TreeHeight);
