namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<EditorTreeNode> BuildProjectItems()
    {
        var title = DocumentFileName;
        var path = string.IsNullOrWhiteSpace(CurrentScenePath) ? title : CurrentScenePath;
        return _projectTreePresenter.Build(title, path, _sceneState.Entities);
    }

    IReadOnlyList<InspectorFieldRow> BuildProjectInspectorFields()
    {
        if (!_editorState.Snapshot.HasSelection || !_projectTreePresenter.IsSceneNode(_editorState.Snapshot.SelectionKey))
            return [];
        var title = DocumentFileName;
        var path = string.IsNullOrWhiteSpace(CurrentScenePath) ? title : CurrentScenePath;
        return [new("名称", title), new("类型", "场景"), new("路径", path)];
    }
}
