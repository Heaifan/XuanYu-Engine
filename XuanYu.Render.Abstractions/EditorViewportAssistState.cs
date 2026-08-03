namespace XuanYu.Render.Abstractions;

// F3-F1：导航 Gizmo 悬停索引（-1=无；0..5=六个端点）——UI 指针流更新，Overlay Pass 高亮。
public readonly record struct EditorViewportAssistState(
    bool ShowGrid = true,
    bool ShowOrigin = true,
    bool ShowWorldAxes = false,
    bool ShowEditorBackground = true,
    int NavGizmoHoverIndex = -1)
{
    public static EditorViewportAssistState Default { get; } = new(true, true, false, true);
}
