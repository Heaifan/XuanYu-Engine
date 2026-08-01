namespace XuanYu.Render.Abstractions;

public readonly record struct EditorViewportAssistState(
    bool ShowGrid = true,
    bool ShowOrigin = true,
    bool ShowWorldAxes = false,
    bool ShowEditorBackground = true)
{
    public static EditorViewportAssistState Default { get; } = new(true, true, false, true);
}
