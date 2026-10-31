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

    // A1 收口：宽度/深度/基础高度一次原子提交——单候选、单验证、单历史节点、
    // 单次 ChangeSequence/ContentChanged；失败整体拒绝零污染。
    // 单字段命令（ResizeMap/SetBaseHeight）保留供未来单字段 Inspector/自动化 API。
    public EngineResult UpdateMapProperties(double widthMeters, double depthMeters, double baseHeightMeters)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "修改地图属性必须在编辑写线程执行。");
        return CommitMapChange(
            map => map with
            {
                SizeMeters = new MapSize(widthMeters, depthMeters),
                Surface = map.Surface with { BaseHeightMeters = baseHeightMeters }
            },
            MapEditReason.MapPropertiesChanged);
    }
}
