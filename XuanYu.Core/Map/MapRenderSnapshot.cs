namespace XuanYu.Core.Map;

// MAP-A-R1-D3/D4：供 Render 消费的最小地图快照（D4 生成网格的唯一数据源）。
// 携带地表参数与环境参数；环境语义：sunDirection = 光线传播方向（从天空射向地面），
// Lambert 渲染使用其反方向（表面指向光源）。
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
