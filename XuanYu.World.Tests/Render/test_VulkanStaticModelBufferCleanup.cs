using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanStaticModelBufferCleanupTests
{
    [Fact]
    public void Map_memory_failure_destroys_buffer_before_freeing_bound_memory()
    {
        var path = Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render",
            "Scene", "Models", "VulkanStaticModelBuffer.cs");
        var source = File.ReadAllText(path);
        var start = source.IndexOf("if (vk.MapMemory", StringComparison.Ordinal);
        var end = source.IndexOf("}", start, StringComparison.Ordinal);
        Assert.True(start >= 0 && end > start, "MapMemory failure branch was not found.");
        var failureBranch = source[start..(end + 1)];
        var destroyBuffer = failureBranch.IndexOf("DestroyBuffer", StringComparison.Ordinal);
        var freeMemory = failureBranch.IndexOf("FreeMemory", StringComparison.Ordinal);

        Assert.True(destroyBuffer >= 0 && freeMemory >= 0, "Failure cleanup is incomplete.");
        Assert.True(destroyBuffer < freeMemory, "Bound memory must outlive its buffer.");
    }
}
