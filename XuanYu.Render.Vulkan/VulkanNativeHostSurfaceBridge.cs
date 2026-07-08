using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Bridge;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
namespace XuanYu.Render.Vulkan;
// VK3-C2-R1：VulkanBridge 生命周期日志经回调进入编辑器日志面板，不再只依赖 Console.WriteLine；
// Attach 失败记录原因后吞掉异常；Swapchain 由 VK4-C 独立 owner 负责，本类只编排生命周期。
public sealed class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    readonly Action<string>? _log;
    Vk? _vk;
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
    VulkanDeviceOwner? _deviceOwner;
    VulkanSwapchainOwner? _swapchainOwner;
    bool _disposed;

    public Instance? Instance => _instanceOwner?.Instance;
    public SurfaceKHR? Surface => _surfaceOwner?.Surface;

    public VulkanNativeHostSurfaceBridge(Action<string>? log = null) => _log = log;

    public void Attach(NativeHostSurfaceHandle handle)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(VulkanNativeHostSurfaceBridge));
        if (_instanceOwner is not null && _surfaceOwner is not null)
            return;

        var ownedVk = _vk is null;
        var vk = _vk ?? Vk.GetApi();
        VulkanInstanceOwner? instance = null;
        VulkanSurfaceOwner? surface = null;
        try
        {
            instance = VulkanInstanceOwner.Create(vk);
            surface = VulkanSurfaceOwner.Create(vk, instance.Instance, handle);
            if (ownedVk) _vk = vk;
            _instanceOwner = instance;
            _surfaceOwner = surface;
            Emit(VulkanBridgeLogFormatter.Attached(handle.Hwnd));
            var selection = VulkanBridgePhysicalDeviceAttachStep.Run(vk, _instanceOwner.Instance, _surfaceOwner.Surface, Emit);
            _deviceOwner = VulkanBridgeDeviceAttachStep.Run(vk, selection, Emit, VulkanSwapchainOwner.DeviceExtensionName);
            if (_deviceOwner is not null)
                _swapchainOwner = VulkanBridgeSwapchainAttachStep.Run(vk, _instanceOwner.Instance,
                    _deviceOwner, _surfaceOwner.Surface, selection, handle.Width, handle.Height, Emit);
        }
        catch (Exception ex)
        {
            surface?.Dispose();
            instance?.Dispose();
            if (ownedVk) vk.Dispose();
            _surfaceOwner = null;
            _instanceOwner = null;
            if (ownedVk) _vk = null;
            Emit(VulkanBridgeLogFormatter.AttachFailed(ex.Message));
        }
    }

    public void Resize(int width, int height)
    {
        if (_instanceOwner is null || _surfaceOwner is null)
        {
            Emit(VulkanBridgeLogFormatter.ResizedSkipped(width, height));
            return;
        }
        Emit(VulkanBridgeLogFormatter.Resized(width, height));
        _swapchainOwner?.Recreate(width, height);
    }

    public void Detach()
    {
        if (_instanceOwner is null && _surfaceOwner is null)
        {
            Emit(VulkanBridgeLogFormatter.DetachedSkipped());
            return;
        }
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
        _vk?.Dispose();
        _vk = null;
    }
    void Emit(string message) => VulkanBridgeLogFormatter.Emit(_log, message);
}
