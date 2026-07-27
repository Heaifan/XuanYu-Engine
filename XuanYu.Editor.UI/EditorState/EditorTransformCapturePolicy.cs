namespace XuanYu.Editor.UI;

public static class EditorTransformCapturePolicy
{
    public static bool CanBeginMoveGizmo(EditorToolSnapshot snapshot) =>
        snapshot.ActiveTool == EditorToolId.Move;

    public static bool ShouldShowMoveGizmo(EditorToolSnapshot snapshot, bool hasSelection) =>
        hasSelection && CanBeginMoveGizmo(snapshot);

    public static bool CanBeginRotateGizmo(EditorToolSnapshot snapshot) =>
        snapshot.ActiveTool == EditorToolId.Rotate;

    public static bool ShouldShowRotateGizmo(EditorToolSnapshot snapshot, bool hasSelection) =>
        hasSelection && CanBeginRotateGizmo(snapshot);

    public static bool CanBeginScaleGizmo(EditorToolSnapshot snapshot) =>
        snapshot.ActiveTool == EditorToolId.Scale;

    public static bool ShouldShowScaleGizmo(EditorToolSnapshot snapshot, bool hasSelection) =>
        hasSelection && CanBeginScaleGizmo(snapshot);
}
