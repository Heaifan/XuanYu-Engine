using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan;

// VK3-C1：NativeHost 生命周期 → Vulkan Instance + Surface 的桥接实现。
// 实现 INativeHostSurfaceBridge：Attach 创建 Instance+Surface，Detach/Dispose 释放，
// Resize 只记日志不重建 Surface（红线：Surface 仅绑定 Attach/Detach）。
// 不接 UI 组合根，不碰 PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
    VulkanInstanceOwner? _instanceOwner;
    VulkanSurfaceOwner? _surfaceOwner;
    bool _disposed;

    public Instance? Instance => _instanceOwner?.Instance;
    public SurfaceKHR? Surface => _surfaceOwner?.Surface;

    public void Attach(NativeHostSurfaceHandle handle)
    {
        if (_instanceOwner is not null) return;
        _instanceOwner = VulkanInstanceOwner.Create();
        _surfaceOwner = VulkanSurfaceOwner.Create(Vk.GetApi(), _instanceOwner.Instance, handle);
        Console.WriteLine(VulkanBridgeLogFormatter.Attached(handle.Hwnd));
    }

    public void Resize(int width, int height) =>
        Console.WriteLine(VulkanBridgeLogFormatter.Resized(width, height));

    public void Detach()
    {
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
    }
}
