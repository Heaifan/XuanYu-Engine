using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanMapLabelCacheLifecycleTests
{
    [Fact]
    public void Retention_set_is_formed_once_from_the_complete_frame_draw_plan()
    {
        var draw = Read("Render", "Scene", "Frame", "VulkanClearFrameOwner.Draw.cs");
        var labels = Read("Render", "Overlay", "LabelOwner", "VulkanClearFrameOwner.DrawMapLabels.cs");
        var retain = draw.IndexOf("_mapLabelTextures?.RetainOnly", StringComparison.Ordinal);
        var loop = draw.IndexOf("foreach (var draw in drawPlan)", StringComparison.Ordinal);

        Assert.True(retain >= 0 && retain < loop);
        Assert.Contains("SelectMany", draw);
        Assert.Contains("LabelBitmaps", draw);
        Assert.DoesNotContain("RetainOnly", labels);
    }

    [Fact]
    public void Descriptor_pool_allows_sets_to_be_freed_after_frame_retirement()
    {
        var pool = Read("Render", "Overlay", "LabelCache", "VulkanMapLabelTextureCache.Descriptors.cs");
        var texture = Read("Render", "Overlay", "LabelCache", "VulkanMapLabelTexture.cs");

        Assert.Contains("FreeDescriptorSetBit", pool);
        Assert.Contains("FreeDescriptorSets", pool);
        Assert.True(texture.IndexOf("_releaseDescriptor", StringComparison.Ordinal) <
                    texture.IndexOf("DestroyImageView", StringComparison.Ordinal));
    }

    static string Read(params string[] path) =>
        File.ReadAllText(Path.Combine(new[] { TestSourceRoot.Root, "XuanYu.Render.Vulkan" }.Concat(path).ToArray()));
}
