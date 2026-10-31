using XuanYu.World;

namespace XuanYu.Editor.UI;

internal sealed class HierarchyTreeProjection
{
    readonly Dictionary<string, EditorTreeNode> _nodes = new(StringComparer.Ordinal);
    readonly HashSet<string> _collapsed = new(StringComparer.Ordinal);
    IReadOnlyList<EditorTreeNode> _allNodes = [];
    public IReadOnlyList<EditorTreeNode> Items { get; private set; } = [];
    public IReadOnlySet<string> CollapsedKeys => _collapsed;

    public IReadOnlyList<EditorTreeNode> Build(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        var live = new HashSet<string>(StringComparer.Ordinal);
        _allNodes = entities.GroupBy(item => item.RegionKey).OrderBy(item => item.Key.ToString())
            .SelectMany(group => BuildRegion(group, live)).ToArray();
        foreach (var key in _nodes.Keys.Where(key => !live.Contains(key)).ToArray()) _nodes.Remove(key);
        Items = TreeGuideBuilder.Visible(_allNodes, _collapsed);
        return Items;
    }

    public bool Toggle(EditorTreeNode node)
    {
        if (!node.CanToggle) return false;
        if (!_collapsed.Add(node.Key)) _collapsed.Remove(node.Key);
        Items = TreeGuideBuilder.Visible(_allNodes, _collapsed);
        return true;
    }

    IEnumerable<EditorTreeNode> BuildRegion(IGrouping<RegionKey, WorldEntitySnapshot> group, HashSet<string> live)
    {
        var region = EditorDisplayText.Region(group.Key);
        yield return Node(live, group.Key.ToString(), region, "Region", $"主世界/{region}", 0, "region");
        foreach (var entity in group.OrderBy(item => item.EntityKey.Value))
            yield return Node(live, entity.EntityKey.ToString(), entity.Name, EditorDisplayText.EntityType(entity.Type),
                $"主世界/{region}/{EditorDisplayText.Entity(entity.EntityKey)}", 1, "entity");
    }

    EditorTreeNode Node(HashSet<string> live, string key, string title, string type, string path, int level, string icon)
    {
        live.Add(key);
        if (!_nodes.TryGetValue(key, out var node)) _nodes.Add(key, node = new(key, title, type, path, level, icon));
        else node.Update(title, type, path, level, icon);
        return node;
    }
}
