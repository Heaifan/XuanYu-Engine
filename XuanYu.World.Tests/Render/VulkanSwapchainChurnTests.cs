using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanSwapchainChurnTests
{
    static string Source(string path)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        return File.ReadAllText(Path.Combine(root, "XuanYu.Render.Vulkan", path));
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
