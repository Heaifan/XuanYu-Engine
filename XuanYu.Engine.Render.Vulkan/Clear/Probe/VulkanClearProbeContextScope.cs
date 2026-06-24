using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Clear;

sealed unsafe class VulkanClearProbeContextScope : IDisposable
{
    readonly Vk _vk;
    Silk.NET.Vulkan.Instance _instance;
    SurfaceKHR _surface;
    Silk.NET.Vulkan.Device _device;
    bool _instOk, _surfOk, _devOk;

    internal nint FnDestroySurface, FnGetCaps, FnGetFmts, FnGetModes, FnSurfaceSupport;
    internal nint FnCreateSwapchain, FnDestroySwapchain, FnGetImages, FnAcquire, FnQueuePresent;

    public VulkanClearProbeContextScope() => _vk = Vk.GetApi();
    public Vk Vk => _vk;
    public Silk.NET.Vulkan.Instance Instance => _instance;
    public Silk.NET.Vulkan.Device Device => _device;
    public SurfaceKHR Surface => _surface;

    public bool CreateInstance()
    {
        var a = Marshal.StringToHGlobalAnsi("XuanYu Engine"); var e = Marshal.StringToHGlobalAnsi("XuanYu Engine");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface"); var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)s, (byte*)w };
            var ai = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = Pack(1, 0, 0) };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &ai, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            if (_vk.CreateInstance(&ci, null, out _instance) != Result.Success) return false;
            _instOk = true;

            FnDestroySurface = LoadInst("vkDestroySurfaceKHR");
            FnGetCaps = LoadInst("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            FnGetFmts = LoadInst("vkGetPhysicalDeviceSurfaceFormatsKHR");
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
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfacePtr>(addr);
            var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hinstance, Hwnd = hwnd };
            fixed (SurfaceKHR* sp = &_surface) _surfOk = fn(_instance, &ci, null, sp) == Result.Success;
            return _surfOk;
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
            _devOk = true;

            FnCreateSwapchain = LoadDev("vkCreateSwapchainKHR");
            FnDestroySwapchain = LoadDev("vkDestroySwapchainKHR");
            FnGetImages = LoadDev("vkGetSwapchainImagesKHR");
            FnAcquire = LoadDev("vkAcquireNextImageKHR");
            FnQueuePresent = LoadDev("vkQueuePresentKHR");
            return true;
        }
        finally { Marshal.FreeHGlobal(ext); }
    }

    nint LoadInst(string n) { var p = Marshal.StringToHGlobalAnsi(n); try { return (nint)_vk.GetInstanceProcAddr(_instance, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }
    nint LoadDev(string n) { var p = Marshal.StringToHGlobalAnsi(n); try { return (nint)_vk.GetDeviceProcAddr(_device, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    public void Dispose()
    {
        if (_devOk && _device.Handle != 0) _vk.DestroyDevice(_device, null);
        if (_surfOk && FnDestroySurface != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySurfacePtr>(FnDestroySurface)(_instance, _surface, null);
        if (_instOk) _vk.DestroyInstance(_instance, null);
    }

    static uint Pack(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate void DestroySurfacePtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
}
