using System.IO;

namespace XuanYu.Core.Tests.Render;

// GRID-RW-1：只保护正式世界线承载路径，禁止恢复全屏片元局部 LOD。
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
    public void Line_shaders_generate_world_lines_without_local_lod()
    {
        var vert = ShaderSource("editor_reference_grid_line.vert");
        var frag = ShaderSource("editor_reference_grid_line.frag");
        Assert.Contains("gl_VertexIndex", vert);
        Assert.Contains("LINES_PER_AXIS = 513", vert);
        Assert.Contains("pc.gridState", vert);
        // CORR2：Major/Minor 与连续 Fade 由世界坐标/距离派生，禁止回到局部 LOD 或硬裁剪。
        Assert.Contains("vMajor", vert);
        Assert.Contains("vFade", vert);
        Assert.Contains("smoothstep(0.30", vert);
        Assert.DoesNotContain("fwidth(", vert + frag);
        Assert.DoesNotContain("log10(", vert + frag);
        Assert.DoesNotContain("bandPass", vert + frag);
        Assert.DoesNotContain("discard;", vert + frag); // 实际 discard 语句必带分号；注释中的禁用词说明不算
    }

    [Fact]
    public void Major_minor_alpha_and_fade_contracts_are_frozen()
    {
        var vert = ShaderSource("editor_reference_grid_line.vert");
        var frag = ShaderSource("editor_reference_grid_line.frag");
        // 用户冻结：Minor α≈0.10、Major α≈0.18；Major 保持更远、地平线附近连续归零。
        Assert.Contains("mix(0.10, 0.18, vMajor)", frag);
        Assert.Contains("smoothstep(0.55 * dMax, 0.85 * dMax, dist)", vert);
        Assert.Contains("smoothstep(0.03, 0.12", vert);
    }

    [Fact]
    public void Reference_grid_pipeline_is_dedicated_line_pass_with_depth_bias()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var grid = File.ReadAllText(Path.Combine(root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs"));
        var line = File.ReadAllText(Path.Combine(root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.GridLine.cs"));
        // 入口（Grid.cs）转交专用工厂并携带两个 Shader 字节码。
        Assert.Contains("CreateReferenceGridLinePass", grid);
        Assert.Contains("ShaderBytecodeGridLineVert.Code", grid);
        Assert.Contains("ShaderBytecodeGridLineFrag.Code", grid);
        // 专用工厂（GridLine.cs）：LineList、Empty-input、负 Depth Bias，不再复用全屏 Pass。
        Assert.Contains("PrimitiveTopology.LineList", line);
        Assert.Contains("VertexBindingDescriptionCount = 0", line);
        Assert.Contains("DepthBiasEnable = true", line);
        Assert.Contains("DepthBiasConstantFactor = -4.0f", line);
        Assert.DoesNotContain("StaticModelVertexBinding", line);
        Assert.DoesNotContain("CreateFullscreenPass(", line); // 带左括号=实际调用；注释说明不算
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
