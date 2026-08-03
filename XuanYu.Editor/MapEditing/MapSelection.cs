using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：地图选择状态。只保存稳定 ID，不保存 UI 控件/列表下标/中文名。
// Region 选择同时携带所属 LayerId（供规范化回退图层选择）。
public sealed record MapSelection(MapSelectionKind Kind, MapLayerId? LayerId, MapRegionId? RegionId)
{
    public static MapSelection None { get; } = new(MapSelectionKind.None, null, null);

    public static MapSelection Map { get; } = new(MapSelectionKind.Map, null, null);

    public static MapSelection Layer(MapLayerId layerId) => new(MapSelectionKind.Layer, layerId, null);

    public static MapSelection Region(MapRegionId regionId, MapLayerId layerId) =>
        new(MapSelectionKind.Region, layerId, regionId);
}
