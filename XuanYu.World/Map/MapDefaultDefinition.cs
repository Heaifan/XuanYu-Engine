using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：默认地图工厂。一次性创建完整地图聚合：
// 10 km × 10 km Flat 地表 + 基础地图层（Base）+ 区域层（Region）+ 空区域集合。
// D2 起 Revision 由编辑会话持有，领域聚合保持纯净。
public static class MapDefaultDefinition
{
    public static MapDefinition CreateDefault() => new(
        MapId.New(),
        "未命名地图",
        new MapSize(10000.0, 10000.0),
        MapCoordinateSystem.ZUpMeter,
        MapSurfaceDefinition.DefaultFlat,
        [
            new MapLayer(MapLayerId.New(), "基础地图", 0, MapLayerKind.Base),
            new MapLayer(MapLayerId.New(), "区域", 1, MapLayerKind.Region)
        ],
        ImmutableArray<MapRegion>.Empty);
}
