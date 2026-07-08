using System;
using System.Text;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

// VK3-B1：Vulkan Instance 持有者。仅创建/释放 Instance，启用 VK_KHR_surface 与 VK_KHR_win32_surface。
// 禁止：Surface / PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed unsafe class VulkanInstanceOwner : IDisposable
{
    readonly Vk _vk;
    Instance _instance;
    bool _disposed;

    VulkanInstanceOwner(Vk vk, Instance instance)
    {
        _vk = vk;
        _instance = instance;
    }

    public Instance Instance => _instance;

    public static VulkanInstanceOwner Create()
    {
        var result = CreateWithResult();
        if (!result.Success)
            throw new InvalidOperationException(VulkanInstanceLogFormatter.CreateFailed(result.ErrorType, result.ErrorMessage));
        return result.Owner!;
    }

    public static VulkanInstanceResult CreateWithResult()
    {
        Vk vk;
        try { vk = Vk.GetApi(); }
        catch (Exception ex)
        {
            var t = ex.GetType().Name;
            Console.WriteLine(VulkanInstanceLogFormatter.CreateFailed(t, ex.Message));
            return new VulkanInstanceResult(false, null, 0, t, ex.Message);
        }
        try
        {
            CreateInstance(vk, out var instance, out var apiVersion);
            var owner = new VulkanInstanceOwner(vk, instance);
            Console.WriteLine(VulkanInstanceLogFormatter.Created(apiVersion));
            return new VulkanInstanceResult(true, owner, apiVersion);
        }
        catch (Exception ex)
        {
            vk.Dispose();
            Console.WriteLine(VulkanInstanceLogFormatter.CreateFailed(ex.GetType().Name, ex.Message));
            return new VulkanInstanceResult(false, null, 0, ex.GetType().Name, ex.Message);
        }
    }

    static void CreateInstance(Vk vk, out Instance instance, out uint apiVersion)
    {
        apiVersion = Vk.Version10;
        var appBytes = Encoding.UTF8.GetBytes("XuanYu Engine\0");
        fixed (byte* appName = appBytes)
        {
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = appName,
                PEngineName = appName,
                ApiVersion = apiVersion
            };
            var surfaceExt = Encoding.UTF8.GetBytes("VK_KHR_surface\0");
            var win32Ext = Encoding.UTF8.GetBytes("VK_KHR_win32_surface\0");
            fixed (byte* pSurface = surfaceExt, pWin32 = win32Ext)
            {
                byte** extPtrs = stackalloc byte*[2];
                extPtrs[0] = pSurface;
                extPtrs[1] = pWin32;
                var createInfo = new InstanceCreateInfo
                {
                    SType = StructureType.InstanceCreateInfo,
                    PApplicationInfo = &appInfo,
                    EnabledExtensionCount = 2,
                    PpEnabledExtensionNames = extPtrs
                };
                if (vk.CreateInstance(&createInfo, null, out instance) != Result.Success)
                    throw new InvalidOperationException("创建 Vulkan Instance 失败");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        var handle = _instance.Handle;
        if (handle != 0) _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
        Console.WriteLine(VulkanInstanceLogFormatter.Disposed(handle));
    }
}
