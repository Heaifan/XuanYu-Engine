using Xunit;

namespace XuanYu.World.Tests.Render;

public sealed class ReverseZDepthPipelineContractTests
{
    [Fact]
    public void DepthClearIsZero()
    {
        var source = Read("XuanYu.Render.Vulkan", "Render", "ClearFrame", "Commands", "VulkanClearFrameOwner.FrameCommands.cs");
        Assert.Contains("Depth = 0.0f", source);
    }

    [Fact]
    public void DefaultDepthCompareIsGreaterOrEqual()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "Owners", "Core", "Depth.cs");
        Assert.Contains("CompareOp.GreaterOrEqual", source);
    }

    [Fact]
    public void TerrainReverseDepthContract()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "Owners", "Content", "Terrain.cs");
        Assert.Contains("CompareOp.GreaterOrEqual", source);
    }

    [Fact]
    public void ReferenceGridReverseDepthContract()
    {
        var source = ReadType("Fullscreen");
        Assert.Contains("CompareOp.GreaterOrEqual", source);
        Assert.Contains("DepthTestEnable = depthTest", source);
        Assert.Contains("DepthWriteEnable = false", source);
    }

    [Fact]
    public void SkyDepthDisabledRegression()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "Owners", "Core", "Sky.cs");
        Assert.Contains("DepthTestEnable = false", source);
        Assert.Contains("DepthWriteEnable = false", source);
    }

    [Fact]
    public void OverlayDepthDisabledRegression()
    {
        var source = Read("XuanYu.Render.Vulkan", "Pipeline", "Owners", "Core", "Depth.cs");
        Assert.Contains("DepthTestEnable = depthTest", source);
        Assert.Contains("DepthWriteEnable = depthWrite", source);
    }

    [Fact]
    public void D32SfloatPreserved()
    {
        var source = Read("XuanYu.Render.Vulkan", "Render", "ClearFrame", "Resources", "VulkanDepthAttachment.cs");
        Assert.Contains("Format.D32Sfloat", source);
    }

    static string Read(params string[] parts)
        => File.ReadAllText(Path.Combine([TestSourceRoot.Root, .. parts]));

    static string ReadType(string prefix)
    {
        var root = Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Pipeline", "Owners");
        var typeDirectory = Path.Combine(root, prefix);
        var files = Directory.Exists(typeDirectory)
            ? Directory.GetFiles(typeDirectory, "*.cs")
            : Directory.GetFiles(root, $"{prefix}*.cs", SearchOption.AllDirectories);
        return string.Join("\n", files.OrderBy(path => path).Select(File.ReadAllText));
    }

}
