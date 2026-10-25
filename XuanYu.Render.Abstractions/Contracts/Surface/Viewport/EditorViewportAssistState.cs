namespace XuanYu.Render.Abstractions;

// 导航 Gizmo 状态只作为 UI 输入事实的渲染投影，不持有相机事实。
public readonly record struct EditorViewportAssistState(
    bool ShowGrid = true,
    bool ShowOrigin = true,
    bool ShowWorldAxes = false,
    bool ShowEditorBackground = true,
    int NavGizmoHoverIndex = -1,
    int NavGizmoActiveIndex = -1,
    int NavGizmoPressedIndex = -1,
    bool NavGizmoCenterHover = false,
    EditorViewPlaneGridKind ViewPlaneGrid = EditorViewPlaneGridKind.None)
{
    public static EditorViewportAssistState Default { get; } = new(true, true, false, true);
}
