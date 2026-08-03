using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：默认图层工厂。新建地图时自动生成：
// "基础地图"层（固定层，不可删除）+ "区域"层（普通区域默认归属）。
public static class MapDefaultLayers
{
    public static ImmutableArray<MapLayer> CreateDefault() =>
    [
        new MapLayer(MapLayerId.New(), "基础地图", 0, true, false, IsFixed: true),
        new MapLayer(MapLayerId.New(), "区域", 1, true, false)
    ];
}
