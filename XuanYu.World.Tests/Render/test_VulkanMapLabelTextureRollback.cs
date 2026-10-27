using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanMapLabelTextureRollbackTests
{
    [Fact]
    public void Image_view_failure_is_inside_image_rollback_scope()
    {
        var source = Read("Render", "Overlay", "LabelCache", "Creation", "VulkanMapLabelTextureCache.Create.cs");
        var create = Section(source, "VulkanMapLabelTexture CreateTexture", "Image CreateImage");
        var view = create.IndexOf("_vk.CreateImageView", StringComparison.Ordinal);
        var attempt = create.IndexOf("try", StringComparison.Ordinal);
        var rollback = create.IndexOf("catch", StringComparison.Ordinal);

        Assert.True(attempt >= 0 && attempt < view && view < rollback);
        Assert.InRange(create.IndexOf("DestroyImageView", rollback), rollback, create.Length);
        Assert.True(create.IndexOf("DestroyImage", rollback) < create.IndexOf("FreeMemory", rollback));
    }

    [Fact]
    public void Image_allocation_failure_destroys_image_before_memory()
    {
        var source = Read("Render", "Overlay", "LabelCache", "Creation", "VulkanMapLabelTextureCache.Create.cs");
        var create = Section(source, "Image CreateImage", "int FindMemoryType");
        var rollback = create.IndexOf("catch", StringComparison.Ordinal);

        Assert.True(rollback >= 0);
        Assert.True(create.IndexOf("DestroyImage", rollback) < create.IndexOf("FreeMemory", rollback));
    }

    [Fact]
    public void Failed_queue_wait_requires_device_idle_confirmation()
    {
        var source = Read("Render", "Overlay", "LabelCache", "Upload", "VulkanMapLabelTextureCache.Upload.cs");
        var upload = Section(source, "void Upload", "void Transition");
        var queueWait = upload.IndexOf("QueueWaitIdle", StringComparison.Ordinal);

        Assert.True(queueWait >= 0);
        Assert.True(upload.IndexOf("DeviceWaitIdle", queueWait, StringComparison.Ordinal) > queueWait);
        Assert.Contains("VulkanLabelUploadCompletionUnknownException", upload);
    }

    static string Read(params string[] path) =>
        File.ReadAllText(Path.Combine(new[] { TestSourceRoot.Root, "XuanYu.Render.Vulkan" }.Concat(path).ToArray()));

    static string Section(string source, string start, string end) =>
        source[source.IndexOf(start, StringComparison.Ordinal)..source.IndexOf(end, StringComparison.Ordinal)];
}
