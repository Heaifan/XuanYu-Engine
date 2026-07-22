namespace XuanYu.Editor.UI;

public static class EditorTransformCapturePolicy
{
    public static bool CanBeginMoveGizmo(EditorToolSnapshot snapshot) =>
        snapshot.ActiveTool == EditorToolId.Move;
}
