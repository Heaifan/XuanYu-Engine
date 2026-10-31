using XuanYu.World;

namespace XuanYu.Editor.UI;

internal sealed class ProjectTreePresenter
{
    readonly Dictionary<string, EditorTreeNode> _nodes = new();
    readonly HashSet<string> _collapsed = new(StringComparer.Ordinal);
    readonly EditorTreeNode _scene = new("scene:current", "未命名场景", "场景", "未命名场景", 0, "project");

    public EditorTreeNode? SelectedItem { get; private set; }
    public IReadOnlySet<string> CollapsedKeys => _collapsed;

    public IReadOnlyList<EditorTreeNode> Build(
        string title, string path, IReadOnlyList<WorldEntitySnapshot> entities)
    {
        var live = new HashSet<string>(StringComparer.Ordinal) { _scene.Key };
        _scene.Update(title, "场景", path, 0, "project");
        _scene.SetTreeState(entities.Count > 0, !_collapsed.Contains(_scene.Key));
        var nodes = new List<EditorTreeNode> { _scene };
        foreach (var entity in entities.OrderBy(item => item.EntityKey.Value))
            AddEntityNode(entity, live, nodes);
        foreach (var key in _nodes.Keys.Where(key => !live.Contains(key)).ToArray()) _nodes.Remove(key);
        return TreeGuideBuilder.Visible(nodes, _collapsed);
    }

    public bool Toggle(EditorTreeNode node)
    {
        if (!node.CanToggle) return false;
        if (!_collapsed.Add(node.Key)) _collapsed.Remove(node.Key);
        return true;
    }

    public bool SetSelectedItem(EditorTreeNode? node)
    {
        if (SelectedItem == node) return false;
        SelectedItem = node;
        return true;
    }

    public bool IsSceneNode(string key) => key == _scene.Key;

    void AddEntityNode(WorldEntitySnapshot entity, HashSet<string> live, List<EditorTreeNode> nodes)
    {
        var key = entity.EntityKey.ToString();
        live.Add(key);
        var path = $"主世界/{EditorDisplayText.Region(entity.RegionKey)}/{EditorDisplayText.Entity(entity.EntityKey)}";
        if (!_nodes.TryGetValue(key, out var node))
            _nodes.Add(key, node = new(key, entity.Name, EditorDisplayText.EntityType(entity.Type), path, 1, "entity"));
        else node.Update(entity.Name, EditorDisplayText.EntityType(entity.Type), path, 1, "entity");
        nodes.Add(node);
    }
}
