namespace XuanYu.World.Tests.Render;

public sealed class TerrainGpuCacheRetirementContractTests
{
    [Fact]
    public void Production_recording_retains_evicted_resources_until_command_buffers_are_replaced()
    {
        var cache = ReadCacheSource();
        var commands = File.ReadAllText(Path.Combine(WorkspaceRoot(),
            "XuanYu.Render.Vulkan", "Render", "ClearFrame", "VulkanClearFrameOwner.Commands.cs"));
        var owner = File.ReadAllText(Path.Combine(WorkspaceRoot(),
            "XuanYu.Render.Vulkan", "Render", "Terrain", "VulkanClearFrameOwner.TerrainCacheLifetime.cs"));

        Assert.Contains("Retire(", cache);
        Assert.Contains("BeginTerrainCommandBufferRecording", commands);
        Assert.Contains("CompleteTerrainCommandBufferRecording", commands);
        Assert.Contains("CancelTerrainCommandBufferRecording", commands);
        Assert.Contains("if (!_renderProjection.HasTerrain) _terrainCache.RetainOnly([]);", owner);
        Assert.DoesNotContain("_resources[key].Dispose();", Member(cache,
            "void Trim(", "public void RetainOnly"));
    }

    static string ReadCacheSource()
    {
        var root = Path.Combine(WorkspaceRoot(), "XuanYu.Render.Vulkan", "Render", "Terrain");
        return File.ReadAllText(Path.Combine(root, "VulkanTerrainGpuCache.cs")) +
            File.ReadAllText(Path.Combine(root, "VulkanTerrainGpuCache.Retirement.cs"));
    }

    static string WorkspaceRoot()
    {
        var sourceRoot = Environment.GetEnvironmentVariable("XYE_WORKSPACE_SOURCE_DIRECTORY");
        Assert.False(string.IsNullOrWhiteSpace(sourceRoot));
        return sourceRoot!;
    }

    static string Member(string source, string start, string end)
    {
        var startIndex = source.IndexOf(start, StringComparison.Ordinal);
        var endIndex = source.IndexOf(end, startIndex, StringComparison.Ordinal);
        Assert.True(startIndex >= 0 && endIndex > startIndex);
        return source[startIndex..endIndex];
    }
}
