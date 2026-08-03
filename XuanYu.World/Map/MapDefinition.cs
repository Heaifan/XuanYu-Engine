using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：完整地图领域聚合（权威根）。D2 起编辑器只允许一个
// CurrentMap（本类型）作为唯一权威状态；持久化 DTO（.xymap）在 D6 升级承载。
public sealed record MapDefinition(
    MapId MapId,
    string DisplayName,
    MapSize SizeMeters,
    MapCoordinateSystem CoordinateSystem,
    MapSurfaceDefinition Surface,
    ImmutableArray<MapLayer> Layers,
    ImmutableArray<MapRegion> Regions,
    long Revision);
