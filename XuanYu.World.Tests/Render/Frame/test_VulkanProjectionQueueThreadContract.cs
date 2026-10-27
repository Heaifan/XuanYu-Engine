using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanProjectionQueueThreadContractTests
{
    [Fact]
    public void Producer_does_not_read_present_owned_frame_state()
    {
        var path = Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render", "Scene",
            "VulkanClearFrameOwner.Scene.cs");
        var source = File.ReadAllText(path);
        var producerStart = source.IndexOf("public void QueueRenderProjection", StringComparison.Ordinal);
        var consumerStart = source.IndexOf("public bool TryApplyPendingRenderProjection", StringComparison.Ordinal);
        Assert.True(producerStart >= 0 && consumerStart > producerStart);
        Assert.DoesNotContain("_frameState", source[producerStart..consumerStart]);
        var consumer = source[consumerStart..];
        Assert.Contains("_frameState.HasProjection", consumer);
        Assert.Contains("_frameState.Projection == projection.Projection", consumer);
    }
}
