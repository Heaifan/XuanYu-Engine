namespace XuanYu.Core.Map;

// MAP-A-R1-D3：供 Render 消费的最小地图快照（D4 生成网格的唯一数据源）。
// 携带地表参数以便 D4 通过同一 MapSurfaceSampler 生成网格，禁止第二套公式。
public readonly record struct MapRenderSnapshot(
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
    public static MapRenderSnapshot Empty { get; } = new("", "", 0, 0, MapSurfaceKind.Flat, 0, 0, 0, 0);

    public bool HasMap => !string.IsNullOrEmpty(MapId);
}
