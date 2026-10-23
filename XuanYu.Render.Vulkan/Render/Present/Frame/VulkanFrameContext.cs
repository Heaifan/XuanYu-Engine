using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render.Frame;

internal sealed class VulkanFrameContext
{
    internal VulkanFrameContext(int frameIndex, Semaphore imageAvailable, Fence graphicsFence)
        => (FrameIndex, ImageAvailable, GraphicsFence) = (frameIndex, imageAvailable, graphicsFence);

    internal int FrameIndex { get; }
    internal Semaphore ImageAvailable { get; }
    internal Fence GraphicsFence { get; }
    internal bool GraphicsPending { get; set; }
    internal bool PresentPending { get; set; }
    internal uint LastImageIndex { get; set; }
    internal uint SwapchainGeneration { get; set; }
    internal long SubmissionSerial { get; set; }
    internal long FrameId { get; set; }
    internal long CameraRevision { get; set; }
}
