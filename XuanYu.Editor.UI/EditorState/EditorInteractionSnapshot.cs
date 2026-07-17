namespace XuanYu.Editor.UI;

public enum EditorInteractionPhase { Idle, Captured }

public sealed record EditorInteractionSnapshot(
    long Revision,
    bool HasCapture,
    long SessionId,
    string OwnerTool,
    string StartSnapshot,
    string Preview,
    EditorInteractionPhase Phase,
    EditorInteractionPointerSnapshot Pointer)
{
    public static EditorInteractionSnapshot Initial { get; } =
        new(1, false, 0, "", "", "", EditorInteractionPhase.Idle,
            EditorInteractionPointerSnapshot.Empty);

    public string PhaseText => Phase == EditorInteractionPhase.Idle ? "空闲" : "捕获中";
}
