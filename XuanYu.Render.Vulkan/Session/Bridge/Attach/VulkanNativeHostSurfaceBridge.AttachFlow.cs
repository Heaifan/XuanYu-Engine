using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Bridge;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    public bool Attach(NativeHostSurfaceHandle handle)
    {
        VerifyOwnerThread();
        if (_disposed) throw new ObjectDisposedException(nameof(VulkanNativeHostSurfaceBridge));
        if (_instanceOwner is not null && _surfaceOwner is not null && !_failed) return true;
        var ownedVk = _vk is null; var vk = _vk ?? Vk.GetApi();
        var events = _rendererEvents ?? RendererEventSessionFactory.TryCreate();
        VulkanInstanceOwner? instance = null; VulkanSurfaceOwner? surface = null;
        VulkanDeviceOwner? device = null; VulkanSwapchainOwner? swapchain = null;
        VulkanRenderSession? session = null;
        try
        {
            instance = VulkanInstanceOwner.Create(vk);
            surface = VulkanSurfaceOwner.Create(vk, instance.Instance, handle);
            var selection = VulkanBridgePhysicalDeviceAttachStep.Run(vk, instance.Instance, surface.Surface, Emit);
            EnsurePresentFenceSupport(vk, instance, selection);
            device = VulkanBridgeDeviceAttachStep.Run(vk, selection, Emit,
                VulkanSwapchainOwner.DeviceExtensionName, enableMaintenance1: true)
                ?? throw new InvalidOperationException("LogicalDevice 创建失败");
            swapchain = VulkanBridgeSwapchainAttachStep.Run(vk, instance.Instance, device,
                surface.Surface, selection!, handle.Width, handle.Height, Emit, events)
                ?? throw new InvalidOperationException("Swapchain 创建失败");
            swapchain.AttachEventSession(events);
            session = VulkanBridgeRenderSessionAttachStep.Run(vk, device, swapchain, selection!, Emit,
                handle, _projectionSource?.RenderProjection)
                ?? throw new InvalidOperationException("RenderSession 创建失败");
            CommitAttach(ownedVk, vk, instance, surface, device, swapchain, session, events);
            Emit(VulkanBridgeLogFormatter.Attached(handle.Hwnd)); return true;
        }
        catch (Exception ex) { return HandleAttachFailure(ownedVk, vk, events, instance, surface, device, swapchain, session, ex); }
    }
}
