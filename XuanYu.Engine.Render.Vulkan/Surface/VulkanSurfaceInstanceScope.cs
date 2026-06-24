using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Surface;

/// <summary>临时 VkInstance 生命周期 + Win32 Surface 创建。仅供 VulkanSurfaceProbe 内部使用。</summary>
sealed unsafe class VulkanSurfaceInstanceScope : IDisposable
{
    readonly Vk _vk;
    Silk.NET.Vulkan.Instance _instance;
    SurfaceKHR _surface;
    bool _instanceCreated, _surfaceCreated;
    CreateWin32SurfaceDelegate? _createSurface;
    DestroySurfaceDelegate? _destroySurface;

    public VulkanSurfaceInstanceScope() => _vk = Vk.GetApi();
    public bool HasSurfaceFunctions => _createSurface is not null;

    public Result CreateInstance()
    {
        var apiVersion = PackApiVersion(1, 0, 0);
        if (_vk.EnumerateInstanceVersion(ref apiVersion) != Result.Success)
            apiVersion = PackApiVersion(1, 0, 0);

        var appName = Marshal.StringToHGlobalAnsi("XuanYu Engine");
        var engineName = Marshal.StringToHGlobalAnsi("XuanYu Engine");
        var surfExt = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var win32Ext = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");

        try
        {
            var exts = stackalloc byte*[] { (byte*)surfExt, (byte*)win32Ext };
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)appName,
                ApplicationVersion = PackApiVersion(0, 0, 1),
                PEngineName = (byte*)engineName,
                EngineVersion = PackApiVersion(0, 0, 1),
                ApiVersion = apiVersion
            };
            var ci = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = 2,
                PpEnabledExtensionNames = exts
            };
            var r = _vk.CreateInstance(&ci, null, out _instance);
            if (r != Result.Success) return r;
            _instanceCreated = true;

            var ca = (nint)_vk.GetInstanceProcAddr(_instance, "vkCreateWin32SurfaceKHR");
            var da = (nint)_vk.GetInstanceProcAddr(_instance, "vkDestroySurfaceKHR");
            if (ca != 0 && da != 0)
            {
                _createSurface = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceDelegate>(ca);
                _destroySurface = Marshal.GetDelegateForFunctionPointer<DestroySurfaceDelegate>(da);
            }
            return Result.Success;
        }
        finally
        {
            Marshal.FreeHGlobal(appName); Marshal.FreeHGlobal(engineName);
            Marshal.FreeHGlobal(surfExt); Marshal.FreeHGlobal(win32Ext);
        }
    }

    public Result CreateWin32Surface(nint hinstance, nint hwnd)
    {
        var info = new Win32SurfaceCreateInfoKHR
        {
            SType = StructureType.Win32SurfaceCreateInfoKhr,
            Hinstance = hinstance, Hwnd = hwnd
        };
        var surface = new SurfaceKHR();
        var r = _createSurface!(_instance, &info, null, &surface);
        if (r == Result.Success) { _surface = surface; _surfaceCreated = true; }
        return r;
    }

    public void Dispose()
    {
        if (_surfaceCreated) _destroySurface!(_instance, _surface, null);
        if (_instanceCreated) _vk.DestroyInstance(_instance, null);
    }

    static uint PackApiVersion(uint major, uint minor, uint patch) =>
        (major << 22) | (minor << 12) | patch;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result CreateWin32SurfaceDelegate(
        Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* p, AllocationCallbacks* a, SurfaceKHR* s);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate void DestroySurfaceDelegate(
        Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
}
