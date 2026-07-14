namespace XuanYu.Editor.UI;

public sealed record EditorSelectionSnapshot(
    long Revision,
    bool HasSelection,
    string SelectionTitle,
    string SelectionSubtitle)
{
    public static EditorSelectionSnapshot Initial { get; } =
        new(1, true, "SampleProject", "项目");
}
