using System.IO;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanVectorOverlayPushConstantContractTests
{
    [Fact]
    public void Analytic_stroke_layout_includes_fragment_push_constant_stage()
    {
        Assert.Equal(ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
            VulkanGraphicsPipelineOwner.PushConstantStages(analyticStroke: true));
        Assert.Equal(ShaderStageFlags.VertexBit,
            VulkanGraphicsPipelineOwner.PushConstantStages(analyticStroke: false));
    }

    [Fact]
    public void Stroke_push_uses_its_own_pipeline_layout_and_fragment_stage()
    {
        var path = Path.Combine(RepositoryRoot, "XuanYu.Render.Vulkan", "Render",
            "Overlay", "VectorDrawing", "VulkanClearFrameOwner.DrawVectorOverlay.cs");
        var source = File.ReadAllText(path);
        Assert.Contains("_vectorStrokePipelineLayout", source);
        Assert.Contains("ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit", source);
    }

    static string RepositoryRoot => Environment.GetEnvironmentVariable("XYE_WORKSPACE_SOURCE_DIRECTORY")
        ?? Path.GetFullPath(TestSourceRoot.Root);
}
