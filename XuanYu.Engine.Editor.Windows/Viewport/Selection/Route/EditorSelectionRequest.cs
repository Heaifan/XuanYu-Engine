using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;

/// <summary>选择请求的输入数据。所有外部依赖通过此记录传入。</summary>
public readonly record struct EditorSelectionRequest(
    string? EntityIdStr,
    EditorSelectionReason Reason,
    WorldState? World);
