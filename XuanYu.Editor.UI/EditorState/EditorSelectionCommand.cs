namespace XuanYu.Editor.UI;

public sealed record SelectEditorItemCommand(
    string Source,
    string Key,
    string Title,
    string Type,
    string Path);

public sealed record ClearEditorSelectionCommand;
