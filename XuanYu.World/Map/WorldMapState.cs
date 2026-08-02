using XuanYu.Core.Map;

namespace XuanYu.World.Map;

// MAP-A-R1-D3：World 地图状态（纯数据 + 有限边界 + 高度查询）。
// 世界坐标语义：X 水平横向、Y 水平纵向、Z 高度；水平面 XY。
// 地图范围：X ∈ [-Width/2, Width/2]、Y ∈ [-Depth/2, Depth/2]（闭区间，边界属于地图）。
public sealed record WorldMapState(
    string MapId,
    string Name,
    double WidthMeters,
    double DepthMeters,
    MapSurfaceKind SurfaceKind,
    double BaseHeightMeters,
    double AmplitudeMeters,
    double WavelengthMeters,
    int Seed)
{
    public bool Contains(double worldX, double worldY) =>
        worldX >= -WidthMeters / 2.0 && worldX <= WidthMeters / 2.0 &&
        worldY >= -DepthMeters / 2.0 && worldY <= DepthMeters / 2.0;

    // 地图外不钳制、不返回虚假零高度：由调用方先判 Contains。
    public double SampleHeight(double worldX, double worldY) =>
        MapSurfaceSampler.SampleHeight(
            SurfaceKind, BaseHeightMeters, AmplitudeMeters, WavelengthMeters,
            Seed, worldX, worldY);

    public bool TryGetSurfaceHeight(double worldX, double worldY, out double surfaceZ)
    {
        if (!Contains(worldX, worldY))
        {
            surfaceZ = 0.0;
            return false;
        }

        surfaceZ = SampleHeight(worldX, worldY);
        return true;
    }
}
