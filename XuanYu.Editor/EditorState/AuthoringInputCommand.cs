namespace XuanYu.Editor;

using XuanYu.Editor.MapEditing;

public sealed record BeginAuthoringInputCommand(AuthoringInputKind Kind);
public sealed record EndAuthoringInputCommand;
