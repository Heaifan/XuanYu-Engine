using Xunit;

namespace XuanYu.World.Tests.Render;

public sealed class ReverseZDepthContractPrototypeTests
{
    [Fact]
    public void ReverseDepthContractIsExplicit()
    {
        var root = Root();
        var clear = Read(root, "XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Commands.cs");
        Assert.Contains("Depth = 0.0f", clear);
        foreach (var file in PipelineFiles(root)) Assert.Contains("CompareOp.GreaterOrEqual", Read(root, file));
    }

    [Fact]
    public void ReverseContractIsDefinedForVulkanNdc()
    {
        Assert.Equal(1, ReversePerspective(0.05, 0.05, 500_000), 12);
        Assert.Equal(0, ReversePerspective(500_000, 0.05, 500_000), 12);
        Assert.True(ReversePerspective(20_000, 0.05, 500_000) < ReversePerspective(10_000, 0.05, 500_000));
    }

    [Fact]
    public void GlFragDepthAuditFindsAllCurrentWriters()
    {
        var root = Root();
        var shaders = Directory.GetFiles(Path.Combine(root, "XuanYu.Render.Vulkan", "Shaders"), "*.frag");
        var writers = shaders.Where(x => Read(x).Contains("gl_FragDepth")).Select(Path.GetFileName).ToArray();
        Assert.Contains("editor_world_reference_grid.frag", writers);
        Assert.Contains("editor_view_plane_grid.frag", writers);
        Assert.Contains("editor_world_axes.frag", writers);
    }

    [Fact]
    public void ReverseMigrationSurfaceIncludesPickingAndUnprojectionInputs()
    {
        var root = Root();
        var sources = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories).Select(Read).ToArray();
        Assert.Contains(sources, x => x.Contains("ToViewProjection"));
        Assert.Contains(sources, x => x.Contains("InverseViewProjection"));
        Assert.Contains(sources, x => x.Contains("ViewportPickingService"));
        Assert.Contains(sources, x => x.Contains("WorldRayFactory"));
    }

    static double ReversePerspective(double z, double n, double f) => (f * n / z - n) / (f - n);
    static string[] PipelineFiles(string root) => [
        "XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Depth.cs",
        "XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Fullscreen.cs",
        "XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.GridLine.cs",
        "XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Terrain.cs"];
    static string Root()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "XuanYu.Render.Vulkan"))) dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("Repository root");
    }
    static string Read(string root, string relative) => File.ReadAllText(Path.Combine(root, relative));
    static string Read(string path) => File.ReadAllText(path);
}
