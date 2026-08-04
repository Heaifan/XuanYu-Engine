using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<string> BuildInspectorFields()
    {
        if (!TrySelectedEntityKey(out var key) || !_sceneState.TryGetEntity(key, out var entity))
            return UiText.ProjectInspectorFields;

        var p = entity.Transform.Position;
        var r = entity.Transform.Rotation;
        var s = entity.Transform.Scale;
        return
        [
            $"名称：{entity.Name}",
            $"类型：{EditorDisplayText.EntityType(entity.Type)}",
            $"实体编号：{EditorDisplayText.Entity(entity.EntityKey)}",
            PathField(entity),
            $"区域：{EditorDisplayText.Region(entity.RegionKey)}",
            $"活动状态：{EditorDisplayText.Activity(entity.Activity)}",
            $"全局位置：{EditorDisplayText.Position(entity.GlobalPosition)}",
            "变换",
            $"位置    X {FormatNumber(p.X)}    Y {FormatNumber(p.Y)}    Z {FormatNumber(p.Z)}",
            $"旋转    X {FormatNumber(r.X)}°    Y {FormatNumber(r.Y)}°    Z {FormatNumber(r.Z)}°",
            $"缩放    X {FormatNumber(s.X)}    Y {FormatNumber(s.Y)}    Z {FormatNumber(s.Z)}"
        ];
    }

    static string PathField(WorldEntitySnapshot entity)
    {
        var region = EditorDisplayText.Region(entity.RegionKey);
        var key = EditorDisplayText.Entity(entity.EntityKey);
        return $"路径：主世界/{region}/{key}";
    }
}
