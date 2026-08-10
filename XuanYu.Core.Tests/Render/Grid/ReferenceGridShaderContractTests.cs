using System.IO;

namespace XuanYu.Core.Tests.Render;

public sealed class ReferenceGridShaderContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string ShaderSource(string name) => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Shaders", name));
    static string PipelineSource() => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs"));

    [Fact]
    public void World_grid_is_fullscreen_frame_step_without_fragment_lod()
    {
        var shader = ShaderSource("editor_world_reference_grid.frag");
        Assert.Contains("pc.gridState.x", shader);
        Assert.Contains("stepMeters = max", shader);
        Assert.Contains("t = -nearWorld.z / rayDirection.z", shader);
        Assert.Contains("worldPosition.x / stepMeters", shader);
        Assert.Contains("worldPosition.y / stepMeters", shader);
        Assert.Contains("fwidth(coordinate)", shader);
        Assert.DoesNotContain("BaseHeight", shader);
        Assert.DoesNotContain("log10", shader);
    }

    [Fact]
    public void World_grid_pipeline_is_depth_independent_fullscreen_pass()
    {
        var source = PipelineSource();
        Assert.Contains("ShaderBytecodeWorldReferenceGridFrag.Code", source);
        Assert.Contains("ShaderBytecodeGridVert.Code", source);
        Assert.Contains("depthTest: false", source);
        Assert.DoesNotContain("CreateReferenceGridLinePass", source);
        Assert.DoesNotContain("ShaderBytecodeGridLine", source);
    }

    [Fact]
    public void World_grid_draw_uses_fullscreen_triangle_and_world_plane_state()
    {
        var source = File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Render", "Grid", "VulkanClearFrameOwner.Grid.cs"));
        Assert.Contains("_referenceGridFrameState.StepMeters", source);
        Assert.Contains("scene[43] = 0.0f", source);
        Assert.Contains("RenderDrawPlan.FullscreenTriangleVertexCount", source);
        Assert.DoesNotContain("ReferenceGridLineVertexCount", source);
    }

    [Fact]
    public void World_axes_remain_the_single_source_of_axis_truth()
    {
        var shader = ShaderSource("editor_world_axes.frag");
        Assert.Contains("AXIS_WIDTH_PX", shader);
        Assert.Contains("fwidth(worldPosition.y)", shader);
        Assert.Contains("fwidth(worldPosition.x)", shader);
    }
}
