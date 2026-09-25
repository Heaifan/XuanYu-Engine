using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult DeleteRegionVertex(MapRegionId regionId, int vertexIndex)
    {
        var region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return Fail("UnknownRegion", "区域不存在。");
        if (region.Vertices.Length <= 3) return Fail("RegionMinimumVertices", "区域至少保留三个顶点。");
        if (vertexIndex < 0 || vertexIndex >= region.Vertices.Length)
            return Fail("InvalidRegionVertex", "区域顶点索引无效。");
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "删除区域顶点必须在编辑写线程执行。");
        return CommitMapChange(
            map => map with { Regions = map.Regions.Replace(region, region with { Vertices = region.Vertices.RemoveAt(vertexIndex) }) },
            MapEditReason.RegionVertexDeleted,
            map => UpsertRegionSpatialIndex(map, regionId));
    }

    public EngineResult DeleteRoad(MapRoadId roadId) => DeleteRoadObject(roadId);

    EngineResult DeleteRoadObject(MapRoadId roadId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "删除道路必须在编辑写线程执行。");
        var road = _currentMap.Roads.FirstOrDefault(item => item.RoadId == roadId);
        if (road is null) return Fail("UnknownRoad", "道路不存在。");
        if (MapLayerRules.Find(_currentMap.Layers, road.LayerId)?.IsLocked == true)
            return Fail("RoadLayerLocked", "道路所属图层已锁定。");
        return CommitMapChange(map => map with { Roads = map.Roads.Remove(road) }, MapEditReason.RoadDeleted);
    }

    public EngineResult DeleteMarker(MapMarkerId markerId)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "删除地图标记必须在编辑写线程执行。");
        var marker = (_currentMap.Markers.IsDefault ? [] : _currentMap.Markers)
            .FirstOrDefault(item => item.MarkerId == markerId);
        if (marker is null) return Fail("UnknownMarker", "地图标记不存在。");
        if (MapLayerRules.Find(_currentMap.Layers, marker.LayerId)?.IsLocked == true || marker.IsLocked)
            return Fail("MarkerLocked", "地图标记已锁定。");
        return CommitMapChange(map => map with { Markers = map.Markers.Remove(marker) }, MapEditReason.MarkerDeleted);
    }
}
