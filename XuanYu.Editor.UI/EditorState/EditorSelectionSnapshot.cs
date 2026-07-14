namespace XuanYu.Editor.UI;

public sealed record EditorSelectionSnapshot(
    long Revision,
    bool HasSelection,
    string SelectionKey,
    string SelectionTitle,
    string SelectionSubtitle,
    string SelectionPath)
{
    public static EditorSelectionSnapshot Initial { get; } =
        new(1, true, "project:root", "SampleProject", "项目", "SampleProject");
}
