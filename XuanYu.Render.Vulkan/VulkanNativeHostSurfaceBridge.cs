using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Bridge;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Session;
namespace XuanYu.Render.Vulkan;
// VK3-C2-R1：Bridge 只编排生命周期，Attach 失败记录原因后吞异常；VK4-D 的 RenderSession 由独立 step 创建，Bridge 仅委托。
public sealed class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    readonly Action<string>? _log;
    Vk? _vk;
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
    VulkanDeviceOwner? _deviceOwner;
    VulkanSwapchainOwner? _swapchainOwner;
    VulkanRenderSession? _renderSession;
    bool _disposed;

    public Instance? Instance => _instanceOwner?.Instance;
    public SurfaceKHR? Surface => _surfaceOwner?.Surface;
    public VulkanNativeHostSurfaceBridge(Action<string>? log = null) => _log = log;

    public void Attach(NativeHostSurfaceHandle handle)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(VulkanNativeHostSurfaceBridge));
        if (_instanceOwner is not null && _surfaceOwner is not null) return;
        var ownedVk = _vk is null;
        var vk = _vk ?? Vk.GetApi();
        VulkanInstanceOwner? instance = null;
        VulkanSurfaceOwner? surface = null;
        try
        {
            instance = VulkanInstanceOwner.Create(vk);
            surface = VulkanSurfaceOwner.Create(vk, instance.Instance, handle);
            if (ownedVk) _vk = vk;
            _instanceOwner = instance; _surfaceOwner = surface;
            Emit(VulkanBridgeLogFormatter.Attached(handle.Hwnd));
            var selection = VulkanBridgePhysicalDeviceAttachStep.Run(vk, _instanceOwner.Instance, _surfaceOwner.Surface, Emit);
            _deviceOwner = VulkanBridgeDeviceAttachStep.Run(vk, selection, Emit, VulkanSwapchainOwner.DeviceExtensionName);
            if (_deviceOwner is not null) _swapchainOwner = VulkanBridgeSwapchainAttachStep.Run(vk, _instanceOwner.Instance, _deviceOwner, _surfaceOwner.Surface, selection, handle.Width, handle.Height, Emit);
            if (_swapchainOwner is not null) _renderSession = VulkanBridgeRenderSessionAttachStep.Run(vk, _deviceOwner, _swapchainOwner, selection, Emit, handle);
        }
        catch (Exception ex)
        {
            surface?.Dispose(); instance?.Dispose();
            if (ownedVk) vk.Dispose();
            _surfaceOwner = null; _instanceOwner = null;
            if (ownedVk) _vk = null;
            Emit(VulkanBridgeLogFormatter.AttachFailed(ex.Message));
        }
    }

    public void Resize(int width, int height)
    {
        if (_instanceOwner is null || _surfaceOwner is null) { Emit(VulkanBridgeLogFormatter.ResizedSkipped(width, height)); return; }
        Emit(VulkanBridgeLogFormatter.Resized(width, height));
        _renderSession?.Resize(width, height);
    }

    public void Detach()
    {
        if (_instanceOwner is null && _surfaceOwner is null) { Emit(VulkanBridgeLogFormatter.DetachedSkipped()); return; }
        _renderSession?.Dispose(); _renderSession = null;
        _swapchainOwner?.Dispose(); _swapchainOwner = null;
        _deviceOwner?.Dispose(); _deviceOwner = null;
        _surfaceOwner?.Dispose(); _surfaceOwner = null;
        Emit(VulkanBridgeLogFormatter.SurfaceDisposed());
        _instanceOwner?.Dispose(); _instanceOwner = null;
        Emit(VulkanBridgeLogFormatter.InstanceDisposed());
        Emit(VulkanBridgeLogFormatter.Detached());
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Detach();
        _vk?.Dispose(); _vk = null;
    }
    void Emit(string message) => VulkanBridgeLogFormatter.Emit(_log, message);
}
