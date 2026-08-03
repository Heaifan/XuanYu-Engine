using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：地图基础属性编辑命令（D2 只实现地图级修改，图层/区域命令属 D4/D5）。
public sealed partial class MapEditSession
{
    public EngineResult RenameMap(string name)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "修改地图名称必须在编辑写线程执行。");
        var trimmed = name?.Trim() ?? "";
        if (trimmed.Length == 0) return Fail("InvalidMapName", "地图名称不能为空。");
        return CommitMapChange(map => map with { DisplayName = trimmed }, MapEditReason.Rename);
    }

    public EngineResult ResizeMap(double widthMeters, double depthMeters)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "修改地图尺寸必须在编辑写线程执行。");
        return CommitMapChange(
            map => map with { SizeMeters = new MapSize(widthMeters, depthMeters) },
            MapEditReason.Resize);
    }

    public EngineResult SetBaseHeight(double baseHeightMeters)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "修改基础高度必须在编辑写线程执行。");
        return CommitMapChange(
            map => map with { Surface = map.Surface with { BaseHeightMeters = baseHeightMeters } },
            MapEditReason.BaseHeightChanged);
    }
}
