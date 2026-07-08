using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan;

// VK3-C1-R2：Vk 所有权收口。Bridge 统一持有并释放 Vk；
// InstanceOwner / SurfaceOwner 仅使用传入的 Vk，不负责 Dispose Vk。
// Dispose 顺序固定：Surface → Instance → Vk。仍不接 UI 组合根，不碰 PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    Vk? _vk;
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
    bool _disposed;

    public Instance? Instance => _instanceOwner?.Instance;
    public SurfaceKHR? Surface => _surfaceOwner?.Surface;

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
            Console.WriteLine(VulkanBridgeLogFormatter.Attached(handle.Hwnd));
        }
        catch
        {
            surface?.Dispose();
            instance?.Dispose();
            if (ownedVk) vk.Dispose();
            _surfaceOwner = null;
            _instanceOwner = null;
            if (ownedVk) _vk = null;
            throw;
        }
    }

    public void Resize(int width, int height)
    {
        if (_instanceOwner is null || _surfaceOwner is null)
        {
            Console.WriteLine(VulkanBridgeLogFormatter.ResizedSkipped(width, height));
            return;
        }
        Console.WriteLine(VulkanBridgeLogFormatter.Resized(width, height));
    }

    public void Detach()
    {
        if (_instanceOwner is null && _surfaceOwner is null)
        {
            Console.WriteLine(VulkanBridgeLogFormatter.DetachedSkipped());
            return;
        }
        _surfaceOwner?.Dispose();
        _surfaceOwner = null;
        _instanceOwner?.Dispose();
        _instanceOwner = null;
        Console.WriteLine(VulkanBridgeLogFormatter.Detached());
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Detach();
        _vk?.Dispose();
        _vk = null;
    }
}
