using XuanYu.Core.Identity;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<EditorTreeNode> BuildHierarchyItems()
    {
        var liveKeys = new HashSet<string>(StringComparer.Ordinal);
        var items = new List<EditorTreeNode>
        {
            Node(liveKeys, "hierarchy:root", "世界根节点", "场景根", "主世界/世界根节点", 0, "world"),
            Node(liveKeys, "hierarchy:camera", "主相机", "相机", "主世界/世界根节点/主相机", 1, "camera"),
            Node(liveKeys, "hierarchy:ground", "地面", "地面", "主世界/世界根节点/地面", 1, "ground")
        };
        foreach (var group in _sceneState.Entities.GroupBy(item => item.RegionKey).OrderBy(item => item.Key.ToString()))
        {
            var region = EditorDisplayText.Region(group.Key);
            items.Add(Node(liveKeys, group.Key.ToString(), region, "Region", $"主世界/{region}", 1, "region"));
            items.AddRange(group.OrderBy(entity => entity.EntityKey.Value).Select(entity => Node(liveKeys,
                entity.EntityKey.ToString(), entity.Name, EditorDisplayText.EntityType(entity.Type),
                $"主世界/{region}/{EditorDisplayText.Entity(entity.EntityKey)}", 2, "entity")));
        }
        PruneHierarchyNodeCache(liveKeys);
        return TreeGuideBuilder.Visible(items, _collapsedHierarchyKeys);
    }

    EditorTreeNode Node(HashSet<string> liveKeys, string key, string title, string type, string path, int level, string icon)
    {
        liveKeys.Add(key);
        if (!_hierarchyNodeCache.TryGetValue(key, out var node))
        {
            node = new EditorTreeNode(key, title, type, path, level, icon);
            _hierarchyNodeCache.Add(key, node);
        }
        else node.Update(title, type, path, level, icon);
        return node;
    }

    void PruneHierarchyNodeCache(HashSet<string> liveKeys)
    {
        foreach (var key in _hierarchyNodeCache.Keys.Where(key => !liveKeys.Contains(key)).ToArray())
            _hierarchyNodeCache.Remove(key);
    }

    IReadOnlyList<string> BuildInspectorFields()
    {
        if (!TrySelectedEntityKey(out var key) || !_sceneState.TryGetEntity(key, out var entity))
        {
            return UiText.ProjectInspectorFields;
        }

        var p = entity.Transform.Position;
        return
        [
            $"名称：{entity.Name}",
            $"类型：{EditorDisplayText.EntityType(entity.Type)}",
            $"实体编号：{EditorDisplayText.Entity(entity.EntityKey)}",
            $"路径：主世界/{EditorDisplayText.Region(entity.RegionKey)}/{EditorDisplayText.Entity(entity.EntityKey)}",
            $"区域：{EditorDisplayText.Region(entity.RegionKey)}",
            $"活动状态：{EditorDisplayText.Activity(entity.Activity)}",
            $"全局位置：{EditorDisplayText.Position(entity.GlobalPosition)}",
            "Transform",
            $"位置    X {p.X:g}    Y {p.Y:g}    Z {p.Z:g}"
        ];
    }

    bool TrySelectedEntityKey(out EntityId key)
    {
        key = EntityId.None;
        var text = SelectionKey;
        return TryEntityKey(text, out key);
    }

    bool TryEntityKey(string text, out EntityId key)
    {
        key = EntityId.None;
        const string prefix = "EntityId(";
        if (!text.StartsWith(prefix, StringComparison.Ordinal) || !text.EndsWith(')')) return false;
        var number = text.Substring(prefix.Length, text.Length - prefix.Length - 1);
        if (!int.TryParse(number, out var value) || value <= 0) return false;
        key = EntityId.FromInt(value);
        return true;
    }

    void RefreshWorldProjectionBindings()
    {
        TraceSelection("RefreshWorldProjectionBindings", 1,
            $"实体数={_sceneState.RenderSnapshot.Entities.Count}");
        SynchronizeSelectionProjection();
        OnPropertyChanged(nameof(HierarchyItems));
        OnPropertyChanged(nameof(InspectorFields));
        OnPropertyChanged(nameof(DebugObjectItems));
        PublishSceneRenderSnapshot();
    }
}
