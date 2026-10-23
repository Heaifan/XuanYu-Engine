using Xunit;

namespace XuanYu.World.Tests.Render;

public sealed class ReverseZDepthPipelineContractTests
{
    [Fact]
    public void DepthClearIsZero()
    {
        var source = Read("XuanYu.Render.Vulkan", "Render", "ClearFrame", "VulkanClearFrameOwner.FrameCommands.cs");
        Assert.Contains("Depth = 0.0f", source);
    }

    [Fact]
    public void DefaultDepthCompareIsGreaterOrEqual()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Depth.cs");
        Assert.Contains("CompareOp.GreaterOrEqual", source);
    }

    [Fact]
    public void TerrainReverseDepthContract()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Terrain.cs");
        Assert.Contains("CompareOp.GreaterOrEqual", source);
    }

    [Fact]
    public void ReferenceGridReverseDepthContract()
    {
        var source = ReadType("VulkanGraphicsPipelineOwner.Fullscreen");
        Assert.Contains("CompareOp.GreaterOrEqual", source);
        Assert.Contains("DepthTestEnable = depthTest", source);
        Assert.Contains("DepthWriteEnable = false", source);
    }

    [Fact]
    public void SkyDepthDisabledRegression()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Sky.cs");
        Assert.Contains("DepthTestEnable = false", source);
        Assert.Contains("DepthWriteEnable = false", source);
    }

    [Fact]
    public void OverlayDepthDisabledRegression()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Depth.cs");
        Assert.Contains("DepthTestEnable = depthTest", source);
        Assert.Contains("DepthWriteEnable = depthWrite", source);
    }

    [Fact]
    public void D32SfloatPreserved()
    {
        var source = Read("XuanYu.Render.Vulkan", "Render", "VulkanDepthAttachment.cs");
        Assert.Contains("Format.D32Sfloat", source);
    }

    static string Read(params string[] parts)
        => File.ReadAllText(Path.Combine([TestSourceRoot.Root, .. parts]));

    static string ReadType(string prefix)
    {
        var root = TestSourceRoot.Root;
        return string.Join("\n", Directory.GetFiles(Path.Combine(root, "XuanYu.Render.Vulkan", "Pipeline"), $"{prefix}*.cs")
            .OrderBy(path => path).Select(File.ReadAllText));
    }

}
