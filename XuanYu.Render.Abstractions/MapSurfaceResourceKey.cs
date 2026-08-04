using XuanYu.Core.Map;

namespace XuanYu.Render.Abstractions;

// MAP-A-R2-D3-A1 收口：GPU 地图资源判等键。
// 只包含会改变地面/边界缓冲内容的字段；SourceChangeSequence、名称、
// Dirty、选择、路径、Undo 状态一律不得进入本键。
public readonly record struct MapSurfaceResourceKey(
    string MapId,
    double WidthMeters,
    double DepthMeters,
    double BaseHeightMeters,
    MapSurfaceKind SurfaceKind,
    double AmplitudeMeters,
    double WavelengthMeters,
    int Seed,
    bool IsVisible)
{
    public static MapSurfaceResourceKey From(MapRenderSnapshot map) => new(
        map.MapId,
        map.WidthMeters,
        map.DepthMeters,
        map.BaseHeightMeters,
        map.SurfaceKind,
        map.AmplitudeMeters,
        map.WavelengthMeters,
        map.Seed,
        map.IsVisible);
}
