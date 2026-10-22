using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanSwapchainChurnTests
{
    static string FindSourceRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            var solution = Path.Combine(directory.FullName, "XuanYu.Engine.slnx");
            var vulkan = Path.Combine(directory.FullName, "XuanYu.Render.Vulkan");
            if (File.Exists(solution) && Directory.Exists(vulkan)) return directory.FullName;
        }
        throw new DirectoryNotFoundException(
            $"Could not locate XuanYu.Engine.slnx above test output: {AppContext.BaseDirectory}");
    }

    static string Source(string path)
    {
        return File.ReadAllText(Path.Combine(FindSourceRoot(), "XuanYu.Render.Vulkan", path));
    }

    [Fact]
    public void SamePhysicalExtentSkipsBeforePresentStop()
    {
        var source = Source(Path.Combine("Session", "VulkanRenderSession.Resize.cs"));
        Assert.True(source.IndexOf("if (IsSameSize", StringComparison.Ordinal) < source.IndexOf("_presentLoop.Stop", StringComparison.Ordinal));
    }

    [Fact]
    public void CameraProjectionDoesNotResizeSurface()
    {
        var source = Source(Path.Combine("Session", "VulkanRenderSession.Resize.cs"));
        var projection = source.IndexOf("public void UpdateRenderProjection", StringComparison.Ordinal);
        Assert.DoesNotContain("Resize(", source[projection..]);
    }
}
