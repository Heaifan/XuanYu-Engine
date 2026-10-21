using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    void EnsurePresentFenceSupport(Vk vk, VulkanInstanceOwner instance,
        VulkanPhysicalDeviceSelection? selection)
    {
        if (selection is null || !VulkanPresentFenceCapability.IsSupported(vk, instance.Instance,
            selection.Handle, instance.Properties2Enabled,
            instance.SurfaceMaintenance1Enabled, Emit))
            throw new InvalidOperationException("Present Fence unavailable; refusing unsafe WSI semaphore reuse");
    }

    bool HandleAttachFailure(bool ownedVk, Vk vk, RendererEventSession? events,
        VulkanInstanceOwner? instance, VulkanSurfaceOwner? surface, VulkanDeviceOwner? device,
        VulkanSwapchainOwner? swapchain, VulkanRenderSession? session, Exception ex)
    {
        var released = RollbackAttach(ownedVk, vk, session, swapchain, device, surface, instance);
        if (released) { CloseDiagnostics(events); _rendererEvents = null; }
        else RetainFailedAttach(ownedVk, vk, instance!, surface!, device!, swapchain!, session, events);
        Emit(VulkanBridgeLogFormatter.AttachFailed(ex.Message)); return false;
    }
}
