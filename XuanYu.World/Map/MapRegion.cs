using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1：区域领域模型（领域权威层）。地图上的二维闭合多边形（水平面坐标）。
// 正式区域天然闭合：顶点按顺序构成多边形边，最后一条边自动连接尾点→首点；
// 顶点列表不重复保存首尾点（首点 != 尾点）。高度 Z 由地表采样取得。
// ImmutableArray 保证 record 结构相等（Round-trip 断言可靠）。
public sealed record MapRegion(
    MapRegionId RegionId,
    MapLayerId LayerId,
    string DisplayName,
    MapRegionKind Kind,
    ImmutableArray<MapPoint> Vertices,
    bool IsVisible = true,
    bool IsLocked = false);
