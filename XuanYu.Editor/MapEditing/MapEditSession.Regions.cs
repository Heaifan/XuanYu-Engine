using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R3-D1：区域正式提交入口，复用统一地图候选/校验/历史管线。
public sealed partial class MapEditSession
{
    public EngineResult CreateRegion(MapRegionDraft draft)
    {
        if (!draft.CanClose) return Fail("TooFewRegionVertices", "区域至少需要三个顶点才能提交。");
        return CreateRegion(draft.Close(MapRegionId.New()));
    }

    public EngineResult CreateRegion(MapRegion region)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "创建区域必须在编辑写线程执行。");
        var layer = MapLayerRules.Find(_currentMap.Layers, region.LayerId);
        if (layer?.IsLocked == true) return Fail("RegionLayerLocked", "区域所属图层已锁定。");
        return CommitMapChange(
            map => map with { Regions = map.Regions.Add(region) },
            MapEditReason.RegionCreated,
            map => UpsertRegionSpatialIndex(map, region.RegionId));
    }

    public EngineResult DeleteRegion(MapRegionId regionId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "删除区域必须在编辑写线程执行。");
        var region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return Fail("UnknownRegion", "区域不存在。");
        var layer = MapLayerRules.Find(_currentMap.Layers, region.LayerId);
        if (layer?.IsLocked == true) return Fail("RegionLayerLocked", "区域所属图层已锁定。");
        return CommitMapChange(
            map => map with { Regions = map.Regions.Remove(region) },
            MapEditReason.RegionDeleted,
            _ => RemoveRegionSpatialIndex(regionId));
    }
}
