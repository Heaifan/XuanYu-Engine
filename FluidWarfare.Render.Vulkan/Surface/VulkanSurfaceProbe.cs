using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Surface;

/// <summary>
/// 使用外部传入的 Windows 原生句柄创建并立即释放 Vulkan Surface。
/// </summary>
public static unsafe class VulkanSurfaceProbe
{
    private const string PlatformText = "Windows";

    public static VulkanSurfaceInfo ProbeWindows(nint hinstance, nint hwnd)
    {
        var stopwatch = Stopwatch.StartNew();
        var instance = default(Silk.NET.Vulkan.Instance);
        var surface = default(SurfaceKHR);
        var instanceCreated = false;
        var surfaceCreated = false;
        Vk? vk = null;

        if (!OperatingSystem.IsWindows())
        {
            stopwatch.Stop();
            return new VulkanSurfaceInfo(
                VulkanSurfaceStatus.UnsupportedPlatform,
                "当前平台不支持 Windows Vulkan Surface 创建。",
                PlatformText,
                false,
                stopwatch.Elapsed.TotalMilliseconds);
        }

        if (hinstance == 0 || hwnd == 0)
        {
            stopwatch.Stop();
            return new VulkanSurfaceInfo(
                VulkanSurfaceStatus.Failed,
                "Windows 原生窗口句柄不可用，无法创建 Vulkan Surface。",
                PlatformText,
                false,
                stopwatch.Elapsed.TotalMilliseconds);
        }

        try
        {
            vk = Vk.GetApi();
            var createInstanceResult = CreateInstance(vk, out instance);
            if (createInstanceResult != Result.Success)
            {
                stopwatch.Stop();
                return Failed($"Vulkan Surface 创建失败：Instance 创建失败：{createInstanceResult}。", stopwatch);
            }

            instanceCreated = true;

            var surfaceCreateInfo = new Win32SurfaceCreateInfoKHR
            {
                SType = StructureType.Win32SurfaceCreateInfoKhr,
                Hinstance = hinstance,
                Hwnd = hwnd
            };

            var createSurfaceAddress = (nint)vk.GetInstanceProcAddr(instance, "vkCreateWin32SurfaceKHR");
            var destroySurfaceAddress = (nint)vk.GetInstanceProcAddr(instance, "vkDestroySurfaceKHR");

            if (createSurfaceAddress == 0 || destroySurfaceAddress == 0)
            {
                stopwatch.Stop();
                return Failed("Vulkan Surface 创建失败：无法加载 Windows Surface 扩展函数。", stopwatch);
            }

            var createSurface = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceDelegate>(
                createSurfaceAddress);

            var createSurfaceResult = createSurface(
                instance,
                &surfaceCreateInfo,
                null,
                &surface);

            if (createSurfaceResult != Result.Success)
            {
                stopwatch.Stop();
                return Failed($"Vulkan Surface 创建失败：{createSurfaceResult}。", stopwatch);
            }

            surfaceCreated = true;
            stopwatch.Stop();
            return new VulkanSurfaceInfo(
                VulkanSurfaceStatus.Created,
                "Vulkan Surface 创建成功，并已立即释放。",
                PlatformText,
                true,
                stopwatch.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            stopwatch.Stop();
            return Failed($"Vulkan Surface 创建失败：{ex.Message}", stopwatch);
        }
        finally
        {
            if (vk is not null && surfaceCreated)
            {
                var destroySurfaceAddress = (nint)vk.GetInstanceProcAddr(instance, "vkDestroySurfaceKHR");
                if (destroySurfaceAddress != 0)
                {
                    var destroySurface = Marshal.GetDelegateForFunctionPointer<DestroySurfaceDelegate>(
                        destroySurfaceAddress);
                    destroySurface(instance, surface, null);
                }
            }

            if (vk is not null && instanceCreated)
            {
                vk.DestroyInstance(instance, null);
            }
        }
    }

    private static Result CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance instance)
    {
        instance = default;

        var apiVersion = PackApiVersion(1, 0, 0);
        if (vk.EnumerateInstanceVersion(ref apiVersion) != Result.Success)
        {
            apiVersion = PackApiVersion(1, 0, 0);
        }

        var appName = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var engineName = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var surfaceExtensionName = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var win32SurfaceExtensionName = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");

        try
        {
            var enabledExtensions = stackalloc byte*[]
            {
                (byte*)surfaceExtensionName,
                (byte*)win32SurfaceExtensionName
            };

            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)appName,
                ApplicationVersion = PackApiVersion(0, 0, 1),
                PEngineName = (byte*)engineName,
                EngineVersion = PackApiVersion(0, 0, 1),
                ApiVersion = apiVersion
            };

            var createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledLayerCount = 0,
                PpEnabledLayerNames = null,
                EnabledExtensionCount = 2,
                PpEnabledExtensionNames = enabledExtensions
            };

            return vk.CreateInstance(&createInfo, null, out instance);
        }
        finally
        {
            Marshal.FreeHGlobal(appName);
            Marshal.FreeHGlobal(engineName);
            Marshal.FreeHGlobal(surfaceExtensionName);
            Marshal.FreeHGlobal(win32SurfaceExtensionName);
        }
    }

    private static VulkanSurfaceInfo Failed(string message, Stopwatch stopwatch)
    {
        return new VulkanSurfaceInfo(
            VulkanSurfaceStatus.Failed,
            message,
            PlatformText,
            true,
            stopwatch.Elapsed.TotalMilliseconds);
    }

    private static uint PackApiVersion(uint major, uint minor, uint patch)
    {
        return (major << 22) | (minor << 12) | patch;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private unsafe delegate Result CreateWin32SurfaceDelegate(
        Silk.NET.Vulkan.Instance instance,
        Win32SurfaceCreateInfoKHR* createInfo,
        AllocationCallbacks* allocator,
        SurfaceKHR* surface);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private unsafe delegate void DestroySurfaceDelegate(
        Silk.NET.Vulkan.Instance instance,
        SurfaceKHR surface,
        AllocationCallbacks* allocator);
}
