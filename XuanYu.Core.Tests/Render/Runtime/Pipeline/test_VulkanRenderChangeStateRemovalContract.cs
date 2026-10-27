using System.IO;

namespace XuanYu.Core.Tests.Render.Vulkan;

public sealed class VulkanRenderChangeStateRemovalContractTests
{
    [Fact]
    public void Production_render_path_does_not_keep_unconsumed_change_state()
    {
        var root = Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render");
        var source = string.Join("\n", Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Select(File.ReadAllText));

        Assert.DoesNotContain("_gpuResourceState", source);
        Assert.DoesNotContain("FrameDirty", source);
        Assert.DoesNotContain("MarkFrameRecorded", source);
        Assert.DoesNotContain("MarkUploadsConsumed", source);
        Assert.DoesNotContain("VulkanGpuResourceState", source);
        Assert.DoesNotContain("VulkanRenderChangeConsumer", source);
        Assert.DoesNotContain("VulkanRenderChangeSet", source);
        Assert.DoesNotContain("VulkanUploadRequirement", source);
        Assert.DoesNotContain("VulkanCacheInvalidation", source);
    }
}
