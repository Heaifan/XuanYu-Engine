using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Swapchain;

/// <summary>临时 Instance + Surface + Device 生命周期 + 函数指针加载。供 VulkanSwapchainProbe 内部使用。</summary>
sealed unsafe class VulkanSwapchainProbeContextScope : IDisposable
{
    readonly Vk _vk;
    Silk.NET.Vulkan.Instance _instance;
    SurfaceKHR _surface;
    Silk.NET.Vulkan.Device _device;
    bool _instCreated, _surfCreated, _devCreated;

    internal nint FnDestroySurface, FnGetCaps, FnGetFormats, FnGetModes, FnSurfaceSupport;
    internal nint FnCreateSwapchain, FnDestroySwapchain, FnGetSwapchainImages;

    public VulkanSwapchainProbeContextScope() => _vk = Vk.GetApi();
    public Vk Vk => _vk;
    public Silk.NET.Vulkan.Instance Instance => _instance;
    public Silk.NET.Vulkan.Device Device => _device;
    public SurfaceKHR Surface => _surface;

    public bool CreateInstance()
    {
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var e = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)s, (byte*)w };
            var appInfo = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = PackApiVersion(1, 0, 0) };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            if (_vk.CreateInstance(&ci, null, out _instance) != Result.Success) return false;
            _instCreated = true;

            FnDestroySurface = LoadInst("vkDestroySurfaceKHR");
            FnGetCaps = LoadInst("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            FnGetFormats = LoadInst("vkGetPhysicalDeviceSurfaceFormatsKHR");
            FnGetModes = LoadInst("vkGetPhysicalDeviceSurfacePresentModesKHR");
            FnSurfaceSupport = LoadInst("vkGetPhysicalDeviceSurfaceSupportKHR");
            return true;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }

    public bool CreateSurface(nint hinstance, nint hwnd)
    {
        var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try
        {
            var addr = (nint)_vk.GetInstanceProcAddr(_instance, (byte*)p);
            if (addr == 0) return false;
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceKHRPtr>(addr);
            var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hinstance, Hwnd = hwnd };
            fixed (SurfaceKHR* ps = &_surface) _surfCreated = fn(_instance, &ci, null, ps) == Result.Success;
            return _surfCreated;
        }
        finally { Marshal.FreeHGlobal(p); }
    }

    public bool CreateDevice(Silk.NET.Vulkan.PhysicalDevice pd, uint qi)
    {
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = qi, QueueCount = 1, PQueuePriorities = &qp };
        var ext = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)ext };
            var ci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts };
            if (_vk.CreateDevice(pd, &ci, null, out _device) != Result.Success) return false;
            _devCreated = true;

            FnCreateSwapchain = LoadDev("vkCreateSwapchainKHR");
            FnDestroySwapchain = LoadDev("vkDestroySwapchainKHR");
            FnGetSwapchainImages = LoadDev("vkGetSwapchainImagesKHR");
            return true;
        }
        finally { Marshal.FreeHGlobal(ext); }
    }

    nint LoadInst(string name) { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)_vk.GetInstanceProcAddr(_instance, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }
    nint LoadDev(string name) { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)_vk.GetDeviceProcAddr(_device, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    public void Dispose()
    {
        if (_devCreated && _device.Handle != 0) _vk.DestroyDevice(_device, null);
        if (_surfCreated && FnDestroySurface != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySurfaceKHRPtr>(FnDestroySurface)(_instance, _surface, null);
        if (_instCreated) _vk.DestroyInstance(_instance, null);
    }

    static uint PackApiVersion(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result CreateWin32SurfaceKHRPtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate void DestroySurfaceKHRPtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
}
