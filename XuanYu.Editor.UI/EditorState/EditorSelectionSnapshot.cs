namespace XuanYu.Editor.UI;

public sealed record EditorSelectionSnapshot(
    long Revision,
    bool HasSelection,
    string SelectionKey,
    string SelectionTitle,
    string SelectionSubtitle)
{
    public static EditorSelectionSnapshot Initial { get; } =
        new(1, true, "项目:SampleProject", "SampleProject", "项目");
}
