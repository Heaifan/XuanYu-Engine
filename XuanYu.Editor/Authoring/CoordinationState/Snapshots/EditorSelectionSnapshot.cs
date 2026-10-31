namespace XuanYu.Editor;

public sealed record EditorSelectionSnapshot(
    long Revision,
    bool HasSelection,
    string SelectionKey,
    string SelectionTitle,
    string SelectionSubtitle,
    string SelectionPath)
{
    public static EditorSelectionSnapshot Initial { get; } =
        new(0, false, "", "", "", "");
}
