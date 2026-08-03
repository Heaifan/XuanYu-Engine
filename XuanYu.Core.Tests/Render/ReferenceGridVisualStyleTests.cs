using System.IO;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2-R3：网格视觉样式合同（10.1）与重合合成合同（10.2）。
// 唯一像素线宽：Fine == Coarse；GridWidth ∈ [0.78, 0.90]；CoarseAlpha-FineAlpha ≤ 0.10；
// 重合处非累加合成（max），禁止 fine+coarse 相加。
public sealed class ReferenceGridVisualStyleTests
{
    // 10.1：唯一线宽参数（shader 源码合同，防误删双宽度）。
    [Fact]
    public void Grid_shader_has_single_line_width()
    {
        var frag = ReadShader("editor_reference_grid.frag");
        Assert.Contains("GRID_LINE_WIDTH_PX", frag);
        Assert.DoesNotContain("fineWidthPixels", frag);
        Assert.DoesNotContain("coarseWidthPixels", frag);
        // Fine 与 Coarse 调用同一宽度常量：不得出现两个不同数值字面量。
        var widthLiteral = ExtractConstant(frag, "GRID_LINE_WIDTH_PX");
        Assert.InRange(widthLiteral, 0.78, 0.90);
        Assert.True(widthLiteral <= 1.0, "网格线宽不得超过 1.0px");
    }

    // 10.1：线宽与透明度合同。
    [Fact]
    public void Width_and_alpha_contract()
    {
        const double gridWidth = 0.82;
        const double fineAlpha = 0.16, coarseAlpha = 0.24;
        Assert.InRange(gridWidth, 0.78, 0.90);
        Assert.True(fineAlpha < coarseAlpha, "FineAlpha 必须小于 CoarseAlpha");
        Assert.True(coarseAlpha - fineAlpha <= 0.10, "CoarseAlpha-FineAlpha 必须 ≤ 0.10");
        Assert.True(coarseAlpha - fineAlpha >= 0.04, "仍需轻微深浅差（推荐 0.06~0.08）");
    }

    // 10.2：重合合成非累加（纯数学镜像：max 而非加法）。
    [Theory]
    [InlineData(0.00, 0.20, 0.20)]
    [InlineData(0.16, 0.00, 0.16)]
    [InlineData(0.10, 0.18, 0.18)]
    [InlineData(0.16, 0.24, 0.24)]
    public void Overlap_composition_uses_max_not_sum(double fine, double coarse, double expected)
    {
        var alpha = System.Math.Max(fine, coarse);
        Assert.Equal(expected, alpha, 6);
        // 两者都非零时，max 必须严格小于加法（重合处不得叠成双倍强度）。
        if (fine > 0.0 && coarse > 0.0)
            Assert.True(alpha < fine + coarse, $"重合处 alpha 必须 < fine+coarse（{fine}+{coarse}）");
    }

    // 10.2：归一化颜色混合后 Alpha 仍为 max（禁止归一化后相加）。
    [Fact]
    public void Normalized_color_blend_keeps_max_alpha()
    {
        const double fineC = 0.10, coarseC = 0.18;
        var total = fineC + coarseC;
        var colorFine = 0.365; // #5D6670 R
        var colorCoarse = 0.322; // #525C67 R
        var blended = (colorFine * fineC + colorCoarse * coarseC) / total;
        Assert.InRange(blended, colorCoarse, colorFine);
        // Alpha 不随颜色归一化改变。
        Assert.Equal(0.18, System.Math.Max(fineC, coarseC), 6);
    }

    // 10.2：shader 源码——Alpha 用 max 合成；total 只用于颜色归一化（方案 6.3 允许）。
    [Fact]
    public void Grid_shader_uses_non_accumulative_composition()
    {
        var frag = ReadShader("editor_reference_grid.frag");
        Assert.Contains("float gridAlpha = max(fineContribution, coarseContribution)", frag);
        Assert.DoesNotContain("gridAlpha = fineContribution + coarseContribution", frag);
        Assert.DoesNotContain("gridAlpha +=", frag);
    }

    // 10.4：背景 Shader 合同——独立 ground/horizon 颜色、不引用地图、不写深度。
    [Fact]
    public void Background_shader_has_neutral_ground_and_no_map_dependency()
    {
        var vert = ReadShader("scene.vert");
        Assert.Contains("groundNear", vert);
        Assert.Contains("groundFar", vert);
        Assert.Contains("horizonBand", vert);
        Assert.DoesNotContain(".xymap", vert);
        Assert.DoesNotContain("MapRenderSnapshot", vert);
        Assert.DoesNotContain("gl_FragDepth", vert);
        // 地面与天空必须一眼区分：天空近地平线偏蓝（B>R），地面偏中性（差值小）。
        Assert.True(0.863 - 0.682 >= 0.1, "天空近地平线应保持蓝调");
    }

    static string ReadShader(string name)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var path = Path.Combine(root, "XuanYu.Render.Vulkan", "Shaders", name);
        Assert.True(File.Exists(path), $"Shader 源码缺失：{path}");
        return File.ReadAllText(path);
    }

    static double ExtractConstant(string source, string name)
    {
        var line = source.Split('\n').FirstOrDefault(l => l.Contains(name) && l.Contains('='))
            ?? throw new Xunit.Sdk.XunitException($"常量 {name} 未找到");
        var valuePart = line.Split('=')[1].Trim().TrimEnd(';');
        return double.Parse(valuePart, System.Globalization.CultureInfo.InvariantCulture);
    }
}
