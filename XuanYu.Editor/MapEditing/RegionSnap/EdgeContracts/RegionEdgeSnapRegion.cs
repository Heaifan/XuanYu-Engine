using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionEdgeSnapRegion(
    MapRegionId RegionId,
    ImmutableArray<RegionEdgeSnapVertex> Vertices);
