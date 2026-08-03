using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：绘制中的区域草稿（未闭合顶点序列）。D5 绘制流程使用；
// 一旦提交（Close）即成为正式 MapRegion——正式区域天然闭合，不带可置 false 的闭合标记。
public sealed record MapRegionDraft(
    MapLayerId LayerId,
    string DisplayName,
    MapRegionKind Kind,
    ImmutableArray<MapPoint> Vertices)
{
    public bool CanClose => Vertices.Length >= 3;

    public MapRegion Close(MapRegionId regionId) =>
        new(regionId, LayerId, DisplayName, Kind, Vertices);
}
