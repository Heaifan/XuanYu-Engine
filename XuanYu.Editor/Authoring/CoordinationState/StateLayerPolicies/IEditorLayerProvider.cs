namespace XuanYu.Editor.Layering;

public interface IEditorLayerProvider
{
    IReadOnlyList<EditorLayerItem> Items { get; }
    string EmptyStateTitle { get; }
    string EmptyStateMessage { get; }
    EditorLayerCommandResult Add();
    EditorLayerCommandResult Delete(string id);
    EditorLayerCommandResult Rename(string id, string name);
    EditorLayerCommandResult SetVisible(string id, bool visible);
    EditorLayerCommandResult SetLocked(string id, bool locked);
    EditorLayerCommandResult SetActive(string id);
    EditorLayerCommandResult Move(string id, int targetIndex);
}
