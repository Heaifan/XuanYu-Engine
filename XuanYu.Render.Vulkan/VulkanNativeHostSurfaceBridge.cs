using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan;

// VK3-C1-R1：为 VulkanNativeHostSurfaceBridge 补生命周期异常安全收口。
// Attach：检查 _disposed、判断双字段已附加、用临时变量先创建、全成功才落字段、失败回滚已创建资源。
// Detach/Resize：未附加时输出跳过日志，避免误导。仍不接 UI 组合根，不碰 PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed class VulkanNativeHostSurfaceBridge : INativeHostSurfaceBridge, IDisposable
{
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

        VulkanInstanceOwner? instance = null;
        VulkanSurfaceOwner? surface = null;
        try
        {
            instance = VulkanInstanceOwner.Create();
            surface = VulkanSurfaceOwner.Create(Vk.GetApi(), instance.Instance, handle);
            _instanceOwner = instance;
            _surfaceOwner = surface;
            Console.WriteLine(VulkanBridgeLogFormatter.Attached(handle.Hwnd));
        }
        catch
        {
            surface?.Dispose();
            instance?.Dispose();
            _surfaceOwner = null;
            _instanceOwner = null;
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
    }
}
