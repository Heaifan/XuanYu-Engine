using System.IO;

namespace XuanYu.World.Tests.Render.MapSurfaceOwner;

public sealed class VulkanMapSurfaceOwnerBoundaryContractTests
{
    [Fact]
    public void Map_surface_state_and_gpu_lifetime_live_in_dedicated_owner()
    {
        var owner = Read("XuanYu.Render.Vulkan", "Render", "World", "Map",
            "VulkanMapSurfaceOwner.MapSurface.cs");
        var coordinator = Read("XuanYu.Render.Vulkan", "Render", "ClearFrame",
            "VulkanClearFrameOwner.cs");
        var lifecycle = Read("XuanYu.Render.Vulkan", "Render", "ClearFrame",
            "VulkanClearFrameOwner.Lifecycle.cs");
        var frameCommands = Read("XuanYu.Render.Vulkan", "Render", "ClearFrame",
            "Commands", "VulkanClearFrameOwner.FrameCommands.cs");
        var drawing = Read("XuanYu.Render.Vulkan", "Render", "World", "Map",
            "VulkanMapSurfaceOwner.Draw.cs");
        var retirement = Read("XuanYu.Render.Vulkan", "Render", "World", "Map",
            "VulkanMapSurfaceOwner.Lifetime.cs");

        Assert.Contains("class VulkanMapSurfaceOwner", owner);
        Assert.Contains("_mapSurfaceFrameVertexBuffers", owner);
        Assert.Contains("_retireGpuResource", owner);
        Assert.DoesNotContain("_mapSurfaceFrameVertexBuffers", coordinator);
        Assert.Contains("new VulkanMapSurfaceOwner", coordinator);
        Assert.Contains("_mapSurface.Dispose()", lifecycle);
        Assert.Contains("_mapSurface.UpdatePatch(CurrentViewProjectionState(), frame.FrameIndex)", frameCommands);
        Assert.Contains("if (!allFramesDrained", frameCommands);
        Assert.Contains("_trackFrameResource(vertexBuffer)", drawing);
        Assert.Contains("_retireGpuResource(buffer)", retirement);
    }

    static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine(RepositoryRoot, Path.Combine(parts)));

    static string RepositoryRoot => Path.GetFullPath(TestSourceRoot.Root);
}
