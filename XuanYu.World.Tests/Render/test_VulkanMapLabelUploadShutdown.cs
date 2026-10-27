namespace XuanYu.World.Tests.Render;

public sealed class VulkanMapLabelUploadShutdownTests
{
    [Fact]
    public void Unknown_upload_is_preflighted_before_device_resources_are_disposed()
    {
        var source = Read("Session", "Runtime", "Lifecycle.cs");
        Assert.True(source.IndexOf("EnsureMapLabelUploadsSafe", StringComparison.Ordinal) <
                    source.IndexOf("_presentLoop.Dispose()", StringComparison.Ordinal));
        Assert.Contains("_disposeBlocked = true", source);
        Assert.Contains("MarkFailed(reason)", source);
        Assert.Contains("return false", source);
    }

    [Fact]
    public void Close_preflight_does_not_retry_a_failed_device_wait()
    {
        var source = Read("Render", "Overlay", "LabelCache", "VulkanMapLabelTextureCache.cs");
        var preflight = source[source.IndexOf("void EnsureUploadQuarantineSafe", StringComparison.Ordinal)..];
        Assert.DoesNotContain("_vk.DeviceWaitIdle", preflight);
        Assert.Contains("VulkanLabelUploadCompletionUnknownException", preflight);
    }

    static string Read(params string[] path) =>
        File.ReadAllText(Path.Combine(new[] { TestSourceRoot.Root, "XuanYu.Render.Vulkan" }.Concat(path).ToArray()));
}
