namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;

/// <summary>
/// Swapchain 创建/重建失败阶段，用于精确诊断。
/// </summary>
public enum VulkanScene3dSwapchainStage
{
    SurfaceCapabilities,
    SurfaceFormats,
    PresentModes,
    CreateSwapchain,
    GetSwapchainImages,
    ColorImageViews,
    DepthAttachments,
    RenderPass,
    Framebuffers,
    CommandPool,
    CommandBuffer,
    Synchronization
}
