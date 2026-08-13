using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D4：默认地图工厂。一次性创建完整地图聚合：
// 10 km × 10 km Flat 地表 + 地面层（Ground）+ 边界层（Boundary）+ 区域 1（Region）+ 空区域集合。
// 默认状态：区域 1 可见、未锁定、当前活动图层；系统图层可见。
public static class MapDefaultDefinition
{
    public static MapDefinition CreateDefault() => new(
        MapId.New(),
        "未命名地图",
        new MapSize(10000.0, 10000.0),
        MapCoordinateSystem.ZUpMeter,
        MapSurfaceDefinition.DefaultFlat,
        [
            new MapLayer(MapLayerId.New(), "地面", 0, MapLayerKind.Ground),
            new MapLayer(MapLayerId.New(), "边界", 1, MapLayerKind.Boundary),
            new MapLayer(MapLayerId.New(), "区域 1", 2, MapLayerKind.Region)
        ],
        ImmutableArray<MapRegion>.Empty,
        ImmutableArray<MapRoad>.Empty,
        ImmutableArray<MapMarker>.Empty);
}
