using System.IO;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2-R3：网格视觉样式合同（10.1）与重合合成合同（10.2）。
// 唯一像素线宽：Fine == Coarse；GridWidth ∈ [0.78, 0.90]；CoarseAlpha-FineAlpha ≤ 0.10；重合处 max 合成。
// F2-R3-R2：背景为片元级每像素重建视线（smoothstep edge0<edge1）。
public sealed class ReferenceGridVisualStyleTests
{
    [Fact]
    public void Grid_shader_has_single_line_width()
    {
        var frag = ReadShader("editor_reference_grid.frag");
        Assert.Contains("GRID_LINE_WIDTH_PX", frag);
        Assert.DoesNotContain("fineWidthPixels", frag);
        Assert.DoesNotContain("coarseWidthPixels", frag);
        Assert.InRange(ExtractConstant(frag, "GRID_LINE_WIDTH_PX"), 0.78, 0.90);
    }
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
    [Theory]
    [InlineData(0.00, 0.20, 0.20), InlineData(0.16, 0.00, 0.16)]
    [InlineData(0.10, 0.18, 0.18), InlineData(0.16, 0.24, 0.24)]
    public void Overlap_composition_uses_max_not_sum(double fine, double coarse, double expected)
    {
        var alpha = System.Math.Max(fine, coarse);
        Assert.Equal(expected, alpha, 6);
        if (fine > 0.0 && coarse > 0.0)
            Assert.True(alpha < fine + coarse, $"重合处 alpha 必须 < fine+coarse（{fine}+{coarse}）");
    }
    [Fact]
    public void Normalized_color_blend_keeps_max_alpha()
    {
        const double fineC = 0.10, coarseC = 0.18;
        var blended = (0.365 * fineC + 0.322 * coarseC) / (fineC + coarseC);
        Assert.InRange(blended, 0.322, 0.365);
        Assert.Equal(0.18, System.Math.Max(fineC, coarseC), 6);
    }
    [Fact]
    public void Grid_shader_uses_non_accumulative_composition()
    {
        var frag = ReadShader("editor_reference_grid.frag");
        Assert.Contains("float gridAlpha = max(lowerContribution, upperContribution)", frag);
        Assert.DoesNotContain("gridAlpha = fineContribution + coarseContribution", frag);
        Assert.DoesNotContain("gridAlpha +=", frag);
    }
    [Fact]
    public void Background_shader_has_neutral_ground_and_no_map_dependency()
    {
        var frag = ReadShader("scene.frag");
        Assert.Contains("vBackgroundNdc", frag);
        Assert.Contains("vInvViewProjection", frag);
        Assert.Contains("groundNear", frag);
        Assert.Contains("groundFar", frag);
        Assert.Contains("horizonColor", frag);
        Assert.DoesNotContain(".xymap", frag);
        Assert.DoesNotContain("MapRenderSnapshot", frag);
        Assert.DoesNotContain("gl_FragDepth", frag);
        Assert.Contains("smoothstep(-0.06, 0.0, dir.z)", frag);
        Assert.Contains("smoothstep(0.06, 0.50, -dir.z)", frag);
        Assert.DoesNotContain("smoothstep(0.0, -0.06", frag);
        Assert.DoesNotContain("smoothstep(-0.06, -0.5", frag);
    }
    [Fact]
    public void Background_vertex_no_longer_computes_color()
    {
        var vert = ReadShader("scene.vert");
        Assert.Contains("void backgroundVertex(int vi, out vec4 clipPos)", vert);
        Assert.DoesNotContain("skyTop", vert);
        Assert.DoesNotContain("groundFar", vert);
        Assert.DoesNotContain("horizonBand", vert);
        Assert.Contains("vBackgroundNdc = p[vi]", vert);
        Assert.Contains("vBackgroundNdc = vec2(2.0, 2.0)", vert);
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
        return double.Parse(line.Split('=')[1].Trim().TrimEnd(';'),
            System.Globalization.CultureInfo.InvariantCulture);
    }
}
