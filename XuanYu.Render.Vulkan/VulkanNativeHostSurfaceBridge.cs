using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Bridge;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan;

// VK-LIFE-1：Attach 全成功后才写入字段；失败按现有释放顺序回滚。
public sealed partial class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    readonly Action<string>? _log;
    Vk? _vk;
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
    VulkanDeviceOwner? _deviceOwner;
    VulkanSwapchainOwner? _swapchainOwner;
    VulkanRenderSession? _renderSession;
    bool _disposed;
    bool _failed;

    public Instance? Instance => _instanceOwner?.Instance;
    public SurfaceKHR? Surface => _surfaceOwner?.Surface;
    public VulkanNativeHostSurfaceBridge(Action<string>? log = null) => _log = log;

    public void Attach(NativeHostSurfaceHandle handle)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(VulkanNativeHostSurfaceBridge));
        if (_instanceOwner is not null && _surfaceOwner is not null && !_failed) return;
        var ownedVk = _vk is null;
        var vk = _vk ?? Vk.GetApi();
        VulkanInstanceOwner? instance = null;
        VulkanSurfaceOwner? surface = null;
        VulkanDeviceOwner? device = null;
        VulkanSwapchainOwner? swapchain = null;
        VulkanRenderSession? session = null;
        try
        {
            instance = VulkanInstanceOwner.Create(vk);
            surface = VulkanSurfaceOwner.Create(vk, instance.Instance, handle);
            var selection = VulkanBridgePhysicalDeviceAttachStep.Run(vk, instance.Instance, surface.Surface, Emit);
            device = VulkanBridgeDeviceAttachStep.Run(vk, selection, Emit, VulkanSwapchainOwner.DeviceExtensionName)
                ?? throw new InvalidOperationException("LogicalDevice 创建失败");
            swapchain = VulkanBridgeSwapchainAttachStep.Run(vk, instance.Instance, device, surface.Surface, selection, handle.Width, handle.Height, Emit)
                ?? throw new InvalidOperationException("Swapchain 创建失败");
            session = VulkanBridgeRenderSessionAttachStep.Run(vk, device, swapchain, selection, Emit, handle)
                ?? throw new InvalidOperationException("RenderSession 创建失败");
            CommitAttach(ownedVk, vk, instance, surface, device, swapchain, session);
            Emit(VulkanBridgeLogFormatter.Attached(handle.Hwnd));
        }
        catch (Exception ex)
        {
            RollbackAttach(ownedVk, vk, session, swapchain, device, surface, instance);
            Emit(VulkanBridgeLogFormatter.AttachFailed(ex.Message));
        }
    }

    public void Resize(int width, int height)
    {
        if (_instanceOwner is null || _surfaceOwner is null || _renderSession is null || _failed)
        {
            Emit(VulkanBridgeLogFormatter.ResizedSkipped(width, height));
            return;
        }
        if (_renderSession.IsFailed)
        {
            _failed = true;
            Emit(VulkanBridgeLogFormatter.SessionFailed(_renderSession.FailureReason ?? "未知原因"));
            return;
        }
        Emit(VulkanBridgeLogFormatter.Resized(width, height));
        if (_renderSession.Resize(width, height)) return;
        _failed = true;
        Emit(VulkanBridgeLogFormatter.ResizeFailed());
        Detach();
    }

    public void Detach()
    {
        if (_instanceOwner is null && _surfaceOwner is null) { Emit(VulkanBridgeLogFormatter.DetachedSkipped()); return; }
        if (_renderSession is not null && !_renderSession.TryDispose())
        {
            _failed = true;
            Emit(VulkanBridgeLogFormatter.DetachBlocked());
            return;
        }
        _renderSession = null;
        _swapchainOwner?.Dispose(); _swapchainOwner = null;
        _deviceOwner?.Dispose(); _deviceOwner = null;
        _surfaceOwner?.Dispose(); _surfaceOwner = null;
        Emit(VulkanBridgeLogFormatter.SurfaceDisposed());
        _instanceOwner?.Dispose(); _instanceOwner = null;
        Emit(VulkanBridgeLogFormatter.InstanceDisposed());
        _failed = false;
        Emit(VulkanBridgeLogFormatter.Detached());
    }

}
