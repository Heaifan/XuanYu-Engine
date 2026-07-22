using XuanYu.Core.Identity;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<EditorTreeNode> BuildHierarchyItems()
    {
        var items = new List<EditorTreeNode>
        {
            new("hierarchy:root", "世界根节点", "场景根", "MainWorld/世界根节点", 0, "world"),
            new("hierarchy:camera", "主相机", "相机", "MainWorld/世界根节点/主相机", 1, "camera"),
            new("hierarchy:ground", "地面", "实体", "MainWorld/世界根节点/地面", 1, "entity")
        };
        items.AddRange(_sceneState.Entities.Select(entity => new EditorTreeNode(
            entity.EntityKey.ToString(),
            entity.Name,
            entity.Type,
            $"MainWorld/{entity.EntityKey}",
            1,
            "entity")));
        return items;
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
            $"路径：MainWorld/{entity.EntityKey}",
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
