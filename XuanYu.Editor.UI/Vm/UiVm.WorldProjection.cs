using XuanYu.Core.Identity;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<EditorTreeNode> BuildHierarchyItems()
    {
        var items = new List<EditorTreeNode>
        {
            Node("hierarchy:root", "世界根节点", "场景根", "MainWorld/世界根节点", 0, "world"),
            Node("hierarchy:camera", "主相机", "相机", "MainWorld/世界根节点/主相机", 1, "camera"),
            Node("hierarchy:ground", "地面", "实体", "MainWorld/世界根节点/地面", 1, "entity")
        };
        foreach (var group in _sceneState.Entities.GroupBy(item => item.RegionKey).OrderBy(item => item.Key.ToString()))
        {
            items.Add(Node(group.Key.ToString(), group.Key.ToString(), "Region", $"MainWorld/{group.Key}", 1, "folder"));
            items.AddRange(group.OrderBy(entity => entity.EntityKey.Value).Select(entity => Node(
                entity.EntityKey.ToString(), entity.Name, entity.Type,
                $"MainWorld/{entity.RegionKey}/{entity.EntityKey}", 2, "entity")));
        }
        return items;
    }

    EditorTreeNode Node(string key, string title, string type, string path, int level, string icon)
    {
        if (!_hierarchyNodeCache.TryGetValue(key, out var node))
        {
            node = new EditorTreeNode(key, title, type, path, level, icon);
            _hierarchyNodeCache.Add(key, node);
        }
        else node.Update(title, type, path, level, icon);
        return node;
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
            $"类型：{entity.Type}",
            $"EntityId：{entity.EntityKey}",
            $"路径：MainWorld/{entity.RegionKey}/{entity.EntityKey}",
            $"Region：{entity.RegionKey}",
            $"活跃状态：{entity.Activity}",
            $"GlobalPosition：X {entity.GlobalPosition.X:g}    Y {entity.GlobalPosition.Y:g}    Z {entity.GlobalPosition.Z:g}",
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
            $"EntityCount={_sceneState.RenderSnapshot.Entities.Count}");
        SynchronizeSelectionProjection();
        OnPropertyChanged(nameof(HierarchyItems));
        OnPropertyChanged(nameof(InspectorFields));
        OnPropertyChanged(nameof(DebugObjectItems));
        PublishSceneRenderSnapshot();
    }
}
