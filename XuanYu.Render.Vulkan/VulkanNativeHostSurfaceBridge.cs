using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan;

// VK3-C2-R1：VulkanBridge 生命周期日志经回调进入编辑器日志面板，
// 不再只依赖 Console.WriteLine；Attach 失败记录原因后吞掉异常，避免编辑器崩溃。
// 仍不碰 PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    readonly Action<string>? _log;
    Vk? _vk;
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
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
            RunDeviceSelection();
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

    // VK4-A：Instance + Surface 已就绪后选择物理设备。仅枚举与选择，不创建 VkDevice / Queue / Swapchain。
    // 选择异常不影响已附加的 Instance + Surface（VK3 契约保持）。
    void RunDeviceSelection()
    {
        if (_vk is null || _instanceOwner is null || _surfaceOwner is null) return;
        try
        {
            VulkanPhysicalDeviceSelector.Select(_vk, _instanceOwner.Instance, _surfaceOwner.Surface, Emit);
        }
        catch (Exception ex)
        {
            Emit($"【VulkanDevice】物理设备选择异常：{ex.Message}；Instance + Surface 保持已附加状态");
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
    }

    public void Detach()
    {
        if (_instanceOwner is null && _surfaceOwner is null)
        {
            Emit(VulkanBridgeLogFormatter.DetachedSkipped());
            return;
        }
        _surfaceOwner?.Dispose();
        _surfaceOwner = null;
        _instanceOwner?.Dispose();
        _instanceOwner = null;
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

    void Emit(string message)
    {
        _log?.Invoke(message);
        Console.WriteLine(message);
    }
}
