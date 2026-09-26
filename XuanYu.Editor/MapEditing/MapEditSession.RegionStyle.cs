using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult SetRegionFillColor(MapRegionId regionId, uint rgb)
    {
        if (!GuardWriteThread())
            return Fail("NotOnWriteThread", "修改区域颜色必须在编辑写线程执行。");

        var region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null)
            return Fail("UnknownRegion", "区域不存在。");
        if (region.IsLocked)
            return Fail("FeatureLocked", "区域已锁定。");
        if (MapLayerRules.Find(_currentMap.Layers, region.LayerId)?.IsLocked == true)
            return Fail("FeatureLayerLocked", "区域所属图层已锁定。");

        var normalized = rgb & 0x00FFFFFF;
        return CommitMapChange(
            map => map with
            {
                Regions = map.Regions.Replace(region, region with { FillColorRgb = normalized })
            },
            MapEditReason.RegionStyleChanged);
    }
}
