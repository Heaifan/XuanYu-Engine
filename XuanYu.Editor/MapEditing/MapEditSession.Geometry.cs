using System.Collections.Immutable;
using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult EditRegionVertices(MapRegionId regionId, ImmutableArray<MapPoint> vertices)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "编辑区域顶点必须在编辑写线程执行。");
        var region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return Fail("UnknownRegion", "区域不存在。");
        if (MapLayerRules.Find(_currentMap.Layers, region.LayerId)?.IsLocked == true)
            return Fail("RegionLayerLocked", "区域所属图层已锁定。");
        return CommitMapChange(
            map => map with { Regions = map.Regions.Replace(region, region with { Vertices = vertices }) },
            MapEditReason.RegionGeometryEdited,
            map => UpsertRegionSpatialIndex(map, regionId));
    }

    public EngineResult EditRoadPoints(MapRoadId roadId, ImmutableArray<MapPoint> points)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "编辑道路节点必须在编辑写线程执行。");
        var road = _currentMap.Roads.FirstOrDefault(item => item.RoadId == roadId);
        if (road is null) return Fail("UnknownRoad", "道路不存在。");
        if (MapLayerRules.Find(_currentMap.Layers, road.LayerId)?.IsLocked == true)
            return Fail("RoadLayerLocked", "道路所属图层已锁定。");
        return CommitMapChange(
            map => map with { Roads = map.Roads.Replace(road, road with { Points = points }) },
            MapEditReason.RoadGeometryEdited);
    }
}
