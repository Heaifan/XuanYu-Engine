using XuanYu.Core.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D4-F4：地形亮度合成——Flat 稳定、缓丘可见明暗差、方向光有真实贡献。
public sealed class MapTerrainBrightnessTests
{
    static MapRenderSnapshot Flat() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", "FlatMap", 2000, 2000,
        MapSurfaceKind.Flat, 5.0, 0.0, 1.0, 1,
        -0.35, -0.55, 0.75, 1.0, 0.35);

    static MapRenderSnapshot Hills() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", "Hills", 2000, 2000,
        MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1,
        -0.35, -0.55, 0.75, 1.0, 0.35);

    [Fact]
    public void Flat_terrain_brightness_is_stable_and_bounded()
    {
        // F4：亮度合成避免过早钳制，Flat 全顶点一致且落在 (0.5, 0.9) 区间。
        var mesh = MapTerrainMeshBuilder.Build(Flat(), segments: 4);
        var first = mesh.Vertices[0].Brightness;
        Assert.InRange(first, 0.5f, 0.9f);
        foreach (var v in mesh.Vertices)
            Assert.Equal(first, v.Brightness, 4);
    }

    [Fact]
    public void Hills_terrain_has_visible_brightness_variation()
    {
        // F4：缓丘受光/背光面必须存在实际亮度差，证明方向光确实参与最终颜色。
        var mesh = MapTerrainMeshBuilder.Build(Hills(), segments: 32);
        var min = mesh.Vertices.Min(v => v.Brightness);
        var max = mesh.Vertices.Max(v => v.Brightness);
        Assert.True(max - min > 0.03f,
            $"缓丘亮度差应可辨：min={min:F4} max={max:F4}");
    }

    [Fact]
    public void Directional_light_contributes_to_final_brightness()
    {
        // 关闭方向光后亮度必须显著下降，证明方向光有真实贡献。
        var sunless = Hills() with { SunIntensity = 0.0 };
        var lit = Hills();
        var dark = MapTerrainMeshBuilder.Build(sunless, segments: 16);
        var bright = MapTerrainMeshBuilder.Build(lit, segments: 16);
        Assert.True(bright.Vertices.Max(v => v.Brightness) >
            dark.Vertices.Max(v => v.Brightness) + 0.05f,
            "方向光应对最终亮度有实际贡献");
    }
}
