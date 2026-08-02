namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地表定义。R1 仅支持 Flat 与 GentleHillsV1（确定性参数化起伏）。
public sealed record MapSurfaceDefinition(
    string Kind,
    double BaseHeightMeters,
    double AmplitudeMeters,
    double WavelengthMeters,
    int Seed)
{
    public static MapSurfaceDefinition DefaultGentleHills { get; } = new(
        MapSurfaceKinds.GentleHillsV1, 0.0, 12.0, 400.0, 1);
}

// R1 已知地表类型清单（D1 合同冻结）。
public static class MapSurfaceKinds
{
    public const string Flat = "Flat";
    public const string GentleHillsV1 = "GentleHillsV1";

    public static bool IsKnown(string? kind) =>
        kind is Flat or GentleHillsV1;
}
