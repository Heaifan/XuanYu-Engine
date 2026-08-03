using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：区域领域模型。地图上的二维闭合多边形（水平面坐标）。
// 顶点只保存 X/Y；高度 Z 由地表采样取得（未来切换高程地表时可重新贴地）。
// ImmutableArray 保证 record 结构相等（Round-trip 断言可靠）。
public sealed record MapRegion(
    MapRegionId RegionId,
    MapLayerId LayerId,
    string DisplayName,
    MapRegionKind Kind,
    ImmutableArray<MapPoint> Vertices,
    bool IsClosed,
    bool IsVisible = true,
    bool IsLocked = false);
