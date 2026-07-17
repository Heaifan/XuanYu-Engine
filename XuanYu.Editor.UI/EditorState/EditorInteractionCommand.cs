namespace XuanYu.Editor.UI;

public sealed record BeginInteractionCommand(string OwnerTool, string StartSnapshot);
public sealed record PreviewInteractionCommand(long SessionId, string OwnerTool, string Preview);
public sealed record CommitInteractionCommand(long SessionId, string OwnerTool);
public sealed record CancelInteractionCommand(long SessionId, string OwnerTool, string Reason);
