using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    void CommitAttach(bool ownedVk, Vk vk, VulkanInstanceOwner instance, VulkanSurfaceOwner surface,
        VulkanDeviceOwner device, VulkanSwapchainOwner swapchain, VulkanRenderSession session,
        RendererEventSession? events)
    {
        if (ownedVk) _vk = vk;
        _instanceOwner = instance;
        _surfaceOwner = surface;
        _deviceOwner = device;
        _swapchainOwner = swapchain;
        _renderSession = session;
        _rendererEvents = events;
        _failed = false;
    }

    bool RollbackAttach(bool ownedVk, Vk vk, VulkanRenderSession? session, VulkanSwapchainOwner? swapchain,
        VulkanDeviceOwner? device, VulkanSurfaceOwner? surface, VulkanInstanceOwner? instance)
    {
        if (session is not null && !session.TryDispose()) return false;
        swapchain?.Dispose();
        device?.Dispose();
        surface?.Dispose();
        instance?.Dispose();
        if (ownedVk) vk.Dispose();
        if (ownedVk) _vk = null;
        return true;
    }

    void RetainFailedAttach(bool ownedVk, Vk vk, VulkanInstanceOwner instance,
        VulkanSurfaceOwner surface, VulkanDeviceOwner device, VulkanSwapchainOwner swapchain,
        VulkanRenderSession? session, RendererEventSession? events)
    {
        if (ownedVk) _vk = vk;
        _instanceOwner = instance; _surfaceOwner = surface; _deviceOwner = device;
        _swapchainOwner = swapchain; _renderSession = session; _rendererEvents = events;
        _failed = true;
    }

    static void CloseDiagnostics(RendererEventSession? events)
    {
        if (events is null) return;
        events.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
