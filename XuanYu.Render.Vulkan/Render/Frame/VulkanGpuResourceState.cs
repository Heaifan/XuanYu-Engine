namespace XuanYu.Render.Vulkan.Render;

internal sealed class VulkanGpuResourceState
{
    public VulkanUploadRequirement UploadRequired { get; private set; }
    public bool FrameDirty { get; private set; }

    public static VulkanGpuResourceState From(VulkanRenderChangeSet changes) => new()
    {
        UploadRequired = changes.UploadRequired,
        FrameDirty = changes.FrameDirty
    };

    public void Apply(VulkanRenderChangeSet changes)
    {
        UploadRequired = changes.UploadRequired;
        FrameDirty = changes.FrameDirty;
    }

    public void MarkUploadsConsumed() => UploadRequired = VulkanUploadRequirement.None;

    public void MarkFrameRecorded() => FrameDirty = false;
}
