namespace XuanYu.Editor.UI;

public sealed record BeginInteractionCommand(
    string OwnerTool,
    string StartSnapshot,
    EditorInteractionPointerSnapshot Pointer);
public sealed record PreviewInteractionCommand(
    long SessionId,
    string OwnerTool,
    string Preview,
    EditorInteractionPointerSnapshot Pointer);
public sealed record CommitInteractionCommand(long SessionId, string OwnerTool, long PointerId);
public sealed record CancelInteractionCommand(long SessionId, string OwnerTool, string Reason);
