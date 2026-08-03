using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：完整地图领域聚合（权威根）。只描述地图内容（纯净、不可变），
// 不承担编辑会话版本/Undo 游标/渲染代数——那些由编辑器会话持有。
// D2 起编辑器只允许一个 CurrentMap（本类型）作为唯一权威状态。
public sealed record MapDefinition(
    MapId MapId,
    string DisplayName,
    MapSize SizeMeters,
    MapCoordinateSystem CoordinateSystem,
    MapSurfaceDefinition Surface,
    ImmutableArray<MapLayer> Layers,
    ImmutableArray<MapRegion> Regions);
