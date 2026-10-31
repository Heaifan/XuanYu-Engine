using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D3：.xymap v1 DTO → 领域聚合投影（场景 mapReference 保活链）。
// v1 无图层/区域（layerReferences 强制空数组），投影补默认两层骨架以满足聚合校验；
// 尺寸/地表/名称/ID 来自文件。schema v2（含图层/区域持久化）属 D6，本桥届时退役。
public static class MapDocumentAggregateBridge
{
    public static MapDefinition ToAggregate(MapDocument doc) => new(
        doc.MapId,
        doc.Name,
        doc.SizeMeters,
        doc.CoordinateSystem,
        doc.Surface,
        MapDefaultDefinition.CreateDefault().Layers,
        ImmutableArray<MapRegion>.Empty);
}
