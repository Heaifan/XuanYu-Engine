using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Handles;

/// <summary>Swapchain 级句柄与函数指针集合。</summary>
sealed record VulkanScene3dSwapchainHandles(
    VulkanScene3dSwapchainFunctions? Functions,
    VulkanScene3dSwapchainResources? Resources,
    nint FnDestroySurface,
    nint FnCreateSwapchain,
    nint FnDestroySwapchain,
    nint FnGetSwapchainImages,
    nint FnAcquireNextImage,
    nint FnQueuePresent,
    nint FnGetCaps,
    nint FnGetFormats,
    nint FnGetModes)
{
    public bool HasSwapchain => Resources is not null;
}
