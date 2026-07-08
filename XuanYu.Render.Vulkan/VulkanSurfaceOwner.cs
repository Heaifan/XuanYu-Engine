using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan;

// VK3-B2：Vulkan Surface 持有者。仅创建/释放 VkSurfaceKHR（Win32），
// 生命周期绑定 NativeHost Attach/Detach，不绑定 Resize。
// 禁止：PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed unsafe class VulkanSurfaceOwner : IDisposable
{
    readonly KhrSurface _khr;
    readonly Instance _instance;
    SurfaceKHR _surface;
    bool _disposed;

    VulkanSurfaceOwner(KhrSurface khr, Instance instance, SurfaceKHR surface)
    {
        _khr = khr;
        _instance = instance;
        _surface = surface;
    }

    public SurfaceKHR Surface => _surface;

    public static VulkanSurfaceOwner Create(Vk vk, Instance instance, NativeHostSurfaceHandle handle)
    {
        var r = CreateWithResult(vk, instance, handle);
        if (!r.Success) throw new InvalidOperationException(
            VulkanSurfaceLogFormatter.CreateFailed(r.ErrorType, r.ErrorMessage));
        return r.Owner!;
    }

    public static VulkanSurfaceResult CreateWithResult(Vk vk, Instance instance, NativeHostSurfaceHandle handle)
    {
        if (!vk.TryGetInstanceExtension(instance, out KhrSurface? khr) || khr is null)
            return new VulkanSurfaceResult(false, null, "缺扩展", "实例未启用 VK_KHR_surface");
        if (!vk.TryGetInstanceExtension(instance, out KhrWin32Surface? win32) || win32 is null)
            return new VulkanSurfaceResult(false, null, "缺扩展", "实例未启用 VK_KHR_win32_surface");
        try
        {
            var info = new Win32SurfaceCreateInfoKHR
            {
                SType = StructureType.Win32SurfaceCreateInfoKhr,
                Hwnd = handle.Hwnd,
                Hinstance = handle.Hinstance
            };
            if (win32.CreateWin32Surface(instance, &info, null, out var surface) != Result.Success)
                return new VulkanSurfaceResult(false, null, "VkResult", "CreateWin32Surface 失败");
            Console.WriteLine(VulkanSurfaceLogFormatter.Created(handle.Hwnd));
            return new VulkanSurfaceResult(true, new VulkanSurfaceOwner(khr, instance, surface));
        }
        catch (Exception ex)
        {
            return new VulkanSurfaceResult(false, null, ex.GetType().Name, ex.Message);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        var handle = _surface.Handle;
        if (handle != 0) _khr.DestroySurface(_instance, _surface, default);
        _surface = default;
        Console.WriteLine(VulkanSurfaceLogFormatter.Disposed(handle));
    }
}
