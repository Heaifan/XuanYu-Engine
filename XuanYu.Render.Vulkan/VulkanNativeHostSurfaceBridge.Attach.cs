using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    void CommitAttach(bool ownedVk, Vk vk, VulkanInstanceOwner instance, VulkanSurfaceOwner surface,
        VulkanDeviceOwner device, VulkanSwapchainOwner swapchain, VulkanRenderSession session)
    {
        if (ownedVk) _vk = vk;
        _instanceOwner = instance;
        _surfaceOwner = surface;
        _deviceOwner = device;
        _swapchainOwner = swapchain;
        _renderSession = session;
        _failed = false;
    }

    void RollbackAttach(bool ownedVk, Vk vk, VulkanRenderSession? session, VulkanSwapchainOwner? swapchain,
        VulkanDeviceOwner? device, VulkanSurfaceOwner? surface, VulkanInstanceOwner? instance)
    {
        if (session is not null && !session.TryDispose()) return;
        swapchain?.Dispose();
        device?.Dispose();
        surface?.Dispose();
        instance?.Dispose();
        if (ownedVk) vk.Dispose();
        if (ownedVk) _vk = null;
    }
}
