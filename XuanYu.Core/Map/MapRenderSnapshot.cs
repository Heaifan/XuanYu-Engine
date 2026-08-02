namespace XuanYu.Core.Map;

// MAP-A-R1-D3/D4：供 Render 消费的最小地图快照（D4 生成网格的唯一数据源）。
// 携带地表参数与环境参数；环境语义：sunDirection = 指向光源方向（光射来方向，
// D1 合同冻结，Z>0 朝上），Lambert 直接使用，不再取反。
public readonly record struct MapRenderSnapshot(
    string MapId,
    string Name,
    double WidthMeters,
    double DepthMeters,
    MapSurfaceKind SurfaceKind,
    double BaseHeightMeters,
    double AmplitudeMeters,
    double WavelengthMeters,
    int Seed,
    double SunDirectionX = 0.0,
    double SunDirectionY = 0.0,
    double SunDirectionZ = 0.0,
    double SunIntensity = 0.0,
    double AmbientIntensity = 0.0)
{
    public static MapRenderSnapshot Empty { get; } = new("", "", 0, 0, MapSurfaceKind.Flat, 0, 0, 0, 0);

    public bool HasMap => !string.IsNullOrEmpty(MapId);
}
