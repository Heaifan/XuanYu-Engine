using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.World;

namespace XuanYu.Editor.UI;

public static class EditorDisplayText
{
    public static string Entity(EntityId id) => id.IsValid ? $"实体编号({id.Value})" : "实体编号(无)";

    public static string Region(RegionKey region) => $"区域 {region.X},{region.Y},{region.Z}";

    public static string Activity(WorldEntityActivity activity) => activity switch
    {
        WorldEntityActivity.Active => "活跃",
        WorldEntityActivity.Dormant => "休眠",
        WorldEntityActivity.Externalized => "外部化",
        _ => "未知"
    };

    public static string EntityType(string type) => type switch
    {
        "MinimalSceneEntity" => "最小场景实体",
        _ => type
    };

    public static string Position(Vector3d p) => $"X {p.X:g}    Y {p.Y:g}    Z {p.Z:g}";
}
