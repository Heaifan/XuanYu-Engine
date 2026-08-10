using System.IO;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2-R2：Shader 合同低层门禁（方案 15.5）。
// 只做防止误删/防退化的字符串检查，不声称视觉正确：
// 网格 Shader 不再包含轴线颜色/原点绘制；不再逐 Fragment 选 LOD；存在 Fine/Coarse 参数；
// 存在方向性 fwidth 密度淡出；深度偏移有 clamp。
public sealed class ReferenceGridShaderContractTests
{
    static string ShaderSource(string name)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var path = Path.Combine(root, "XuanYu.Render.Vulkan", "Shaders", name);
        Assert.True(File.Exists(path), $"Shader 源码缺失：{path}");
        return File.ReadAllText(path);
    }

    [Fact]
    public void Grid_shader_has_no_axis_or_origin_drawing()
    {
        var frag = ShaderSource("editor_reference_grid.frag");
        // 轴线/原点已移入独立 Pass（WorldAxes/WorldOrigin）。
        Assert.DoesNotContain("axisXColor", frag);
        Assert.DoesNotContain("axisYColor", frag);
        Assert.DoesNotContain("originColor", frag);
        Assert.DoesNotContain("originMark", frag);
    }

    [Fact]
    public void Grid_shader_uses_local_decade_lod()
    {
        var frag = ShaderSource("editor_reference_grid.frag");
        Assert.Contains("worldPerPixel", frag);
        Assert.Contains("worldPerPixelX", frag);
        Assert.Contains("worldPerPixelY", frag);
        Assert.Contains("sqrt(worldPerPixelX * worldPerPixelY)", frag);
        Assert.Contains("log10Value(idealSpacing)", frag);
        Assert.Contains("levelLine", frag);
        Assert.Contains("return max(xLine, yLine)", frag);
        Assert.Contains("projectedCellPixels", frag);
        Assert.Contains("bandPass", frag);
        Assert.DoesNotContain("decadePhase", frag);
        Assert.DoesNotContain("float fineSpacing", frag);
        Assert.DoesNotContain("pc.gridScale.x", frag);
        // 不再由 Fragment 自己乘 36 选层级。
        Assert.DoesNotContain("36.0", frag);
    }

    [Fact]
    public void Grid_shader_depth_bias_is_clamped()
    {
        var frag = ShaderSource("editor_reference_grid.frag");
        Assert.Contains("clamp(fwidth(depth) * DEPTH_BIAS_FACTOR", frag);
        Assert.Contains("MIN_DEPTH_BIAS", frag);
        Assert.Contains("MAX_DEPTH_BIAS", frag);
    }

    [Fact]
    public void Grid_shader_is_infinite_and_keeps_base_height()
    {
        var frag = ShaderSource("editor_reference_grid.frag");
        Assert.DoesNotContain("mapFade", frag);
        Assert.DoesNotContain("abs(worldPosition.x) - pc.mapBounds.x", frag);
        Assert.DoesNotContain("abs(worldPosition.y) - pc.mapBounds.y", frag);
        Assert.Contains("pc.mapBounds.z", frag);
    }

    [Fact]
    public void World_axes_shader_is_single_source_of_axis_truth()
    {
        var frag = ShaderSource("editor_world_axes.frag");
        Assert.Contains("AXIS_WIDTH_PX", frag);
        Assert.Contains("fwidth(worldPosition.y)", frag);   // X 轴用 y 方向导数
        Assert.Contains("fwidth(worldPosition.x)", frag);   // Y 轴用 x 方向导数
    }

    [Fact]
    public void World_origin_shader_draws_origin_mark_only()
    {
        var frag = ShaderSource("editor_world_origin.frag");
        Assert.Contains("CROSS_HALF_LEN", frag);
        Assert.DoesNotContain("axisXColor", frag);
        Assert.DoesNotContain("gl_FragDepth", frag);
    }

    [Fact]
    public void World_origin_pipeline_is_depth_disabled_overlay()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs"));
        var start = source.IndexOf("CreateWorldOrigin", StringComparison.Ordinal);
        Assert.True(start >= 0, "WorldOrigin 管线工厂缺失");
        var call = source[start..];
        Assert.Contains("depthTest: false", call);
    }
}
