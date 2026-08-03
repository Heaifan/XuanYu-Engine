using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：地图编辑低频事件参数（禁止记录鼠标移动/Hover/每帧渲染）。
public sealed record MapContentChangedEventArgs(
    MapDefinition CurrentMap,
    long ChangeSequence,
    MapEditReason Reason);

public sealed record MapSelectionChangedEventArgs(MapSelection Selection);

public sealed record MapDirtyChangedEventArgs(bool IsDirty);

public sealed record MapHistoryAvailabilityChangedEventArgs(bool CanUndo, bool CanRedo);
