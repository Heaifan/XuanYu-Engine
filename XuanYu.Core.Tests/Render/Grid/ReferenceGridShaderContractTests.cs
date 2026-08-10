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
        Assert.DoesNotContain("fwidth(", vert + frag);
        Assert.DoesNotContain("log10(", vert + frag);
        Assert.DoesNotContain("bandPass", vert + frag);
        Assert.DoesNotContain("discard", frag);
    }

    [Fact]
    public void Reference_grid_pipeline_uses_line_list()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var path = Path.Combine(root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs");
        var source = File.ReadAllText(path);
        Assert.Contains("ShaderBytecodeGridLineVert.Code", source);
        Assert.Contains("ShaderBytecodeGridLineFrag.Code", source);
        Assert.Contains("PrimitiveTopology.LineList", source);
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
