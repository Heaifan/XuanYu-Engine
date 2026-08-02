namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：环境定义。D2 只保存与校验，不渲染。
// sunDirection 指向光源方向（光射来方向），有限且非零；符号语义由 D4 渲染消费时裁定。
public sealed record MapEnvironmentDefinition(
    string SkyPreset,
    MapVector3 SunDirection,
    double SunIntensity,
    double AmbientIntensity)
{
    public static MapEnvironmentDefinition DefaultClearDay { get; } = new(
        MapSkyPresets.ClearDayV1, new MapVector3(-0.35, -0.55, 0.75), 1.0, 0.35);
}

// R1 已知天空预设清单（D1 合同冻结）。
public static class MapSkyPresets
{
    public const string ClearDayV1 = "ClearDayV1";

    public static bool IsKnown(string? preset) => preset is ClearDayV1;
}
