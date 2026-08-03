using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：地图历史条目（不可变快照）。MapDefinition 与 ImmutableArray
// 不可变，可安全保存引用；禁止保存 JSON/文件/UI/Vulkan/可变集合。
public sealed record MapHistoryEntry(
    MapDefinition Before,
    MapDefinition After,
    MapEditReason Reason);
