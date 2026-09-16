using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult RenameRoad(MapRoadId roadId, string name)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "重命名道路必须在编辑写线程执行。");
        var road = _currentMap.Roads.FirstOrDefault(item => item.RoadId == roadId);
        if (road is null) return Fail("UnknownRoad", "道路不存在。");
        return RenameFeature(name, road.IsLocked, road.LayerId, MapEditReason.RoadRenamed,
            map => map with { Roads = map.Roads.Replace(road, road with { DisplayName = name.Trim() }) });
    }

    public EngineResult RenameRegion(MapRegionId regionId, string name)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "重命名区域必须在编辑写线程执行。");
        var region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return Fail("UnknownRegion", "区域不存在。");
        return RenameFeature(name, region.IsLocked, region.LayerId, MapEditReason.RegionRenamed,
            map => map with { Regions = map.Regions.Replace(region, region with { DisplayName = name.Trim() }) });
    }

    public EngineResult RenameMarker(MapMarkerId markerId, string name)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "重命名地图标记必须在编辑写线程执行。");
        var marker = (_currentMap.Markers.IsDefault ? [] : _currentMap.Markers).FirstOrDefault(item => item.MarkerId == markerId);
        if (marker is null) return Fail("UnknownMarker", "地图标记不存在。");
        return RenameFeature(name, marker.IsLocked, marker.LayerId, MapEditReason.MarkerRenamed,
            map => map with { Markers = map.Markers.Replace(marker, marker with { DisplayName = name.Trim() }) });
    }

    EngineResult RenameFeature(string name, bool locked, MapLayerId layerId, MapEditReason reason,
        Func<MapDefinition, MapDefinition> mutation)
    {
        if (locked) return Fail("FeatureLocked", "要素已锁定。");
        if (MapLayerRules.Find(_currentMap.Layers, layerId)?.IsLocked == true) return Fail("FeatureLayerLocked", "要素所属图层已锁定。");
        if (MapLayerRules.ValidateName(name) is { } error) return Fail("InvalidFeatureName", error);
        return CommitMapChange(mutation, reason);
    }
}
