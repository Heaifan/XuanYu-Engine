using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<InspectorFieldRow> BuildInspectorFields()
    {
        if (SelectedDataset is { } dataset)
            return [new("名称", dataset.Name), new("类型", dataset.Type), new("数据集 ID", dataset.Id),
                new("状态", dataset.Status), new("可见", dataset.IsVisible ? "是" : "否"), new("锁定", dataset.IsLocked ? "是" : "否")];
        if (!TrySelectedEntityKey(out var key) || !_sceneState.TryGetEntity(key, out var entity))
            return UiText.ProjectInspectorFields;

        var p = entity.Transform.Position;
        var r = entity.Transform.Rotation;
        var s = entity.Transform.Scale;
        return
        [
            new("名称", entity.Name),
            new("类型", EditorDisplayText.EntityType(entity.Type)),
            new("实体编号", EditorDisplayText.Entity(entity.EntityKey)),
            new("路径", PathField(entity)),
            new("区域", EditorDisplayText.Region(entity.RegionKey)),
            new("活动状态", EditorDisplayText.Activity(entity.Activity)),
            new("全局位置", EditorDisplayText.Position(entity.GlobalPosition)),
            new("变换", "", IsGroupHeader: true),
            new("位置", $"X {FormatNumber(p.X)}    Y {FormatNumber(p.Y)}    Z {FormatNumber(p.Z)}"),
            new("旋转", $"X {FormatNumber(r.X)}°    Y {FormatNumber(r.Y)}°    Z {FormatNumber(r.Z)}°"),
            new("缩放", $"X {FormatNumber(s.X)}    Y {FormatNumber(s.Y)}    Z {FormatNumber(s.Z)}")
        ];
    }

    static string PathField(WorldEntitySnapshot entity)
    {
        var region = EditorDisplayText.Region(entity.RegionKey);
        var key = EditorDisplayText.Entity(entity.EntityKey);
        return $"主世界/{region}/{key}";
    }
}
