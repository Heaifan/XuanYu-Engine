using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;

/// <summary>选择路由的结果。Shell 用此结果应用 UI 展示。</summary>
public sealed record EditorSelectionRouteResult(
    WorldEntityInfo? Entity,
    EditorSelectionReason Reason,
    bool IsChanged);
