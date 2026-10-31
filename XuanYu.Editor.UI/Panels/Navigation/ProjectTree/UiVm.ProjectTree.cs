namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly Dictionary<string, EditorTreeNode> _projectNodeCache = new();
    readonly EditorTreeNode _currentSceneNode =
        new("scene:current", "未命名场景", "场景", "未命名场景", 0, "project");

    IReadOnlyList<EditorTreeNode> BuildProjectItems()
    {
        var title = DocumentFileName;
        var path = string.IsNullOrWhiteSpace(CurrentScenePath) ? title : CurrentScenePath;
        var entities = _sceneState.Entities.OrderBy(entity => entity.EntityKey.Value).ToArray();
        var liveKeys = new HashSet<string>(StringComparer.Ordinal) { _currentSceneNode.Key };
        _currentSceneNode.Update(title, "场景", path, 0, "project");
        _currentSceneNode.SetTreeState(entities.Length > 0,
            !_collapsedProjectKeys.Contains(_currentSceneNode.Key));
        var items = new List<EditorTreeNode> { _currentSceneNode };
        foreach (var entity in entities)
        {
            var key = entity.EntityKey.ToString();
            liveKeys.Add(key);
            if (!_projectNodeCache.TryGetValue(key, out var node))
            {
                node = new EditorTreeNode(key, entity.Name,
                    EditorDisplayText.EntityType(entity.Type),
                    $"主世界/{EditorDisplayText.Region(entity.RegionKey)}/{EditorDisplayText.Entity(entity.EntityKey)}",
                    1, "entity");
                _projectNodeCache.Add(key, node);
            }
            else
            {
                node.Update(entity.Name, EditorDisplayText.EntityType(entity.Type),
                    $"主世界/{EditorDisplayText.Region(entity.RegionKey)}/{EditorDisplayText.Entity(entity.EntityKey)}",
                    1, "entity");
            }
            items.Add(node);
        }
        foreach (var key in _projectNodeCache.Keys.Where(key => !liveKeys.Contains(key)).ToArray())
            _projectNodeCache.Remove(key);
        return TreeGuideBuilder.Visible(items, _collapsedProjectKeys);
    }

    IReadOnlyList<InspectorFieldRow> BuildProjectInspectorFields()
    {
        if (!_editorState.Snapshot.HasSelection || _editorState.Snapshot.SelectionKey != _currentSceneNode.Key)
            return [];
        var title = DocumentFileName;
        var path = string.IsNullOrWhiteSpace(CurrentScenePath) ? title : CurrentScenePath;
        return [new("名称", title), new("类型", "场景"), new("路径", path)];
    }
}
