using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Context;

sealed unsafe class VulkanRenderContextSetup
{
    Vk? _vk;
    Silk.NET.Vulkan.Instance _instance;

    public void SetVk(Vk vk) => _vk = vk;
    public void SetInstance(Silk.NET.Vulkan.Instance inst) => _instance = inst;

    public bool CreateInstance(out Silk.NET.Vulkan.Instance inst, out string apiVersionText)
    {
        inst = default; apiVersionText = "";
        var apiVersion = Pack(1, 0, 0);
        if (_vk!.EnumerateInstanceVersion(ref apiVersion) != Result.Success) apiVersion = Pack(1, 0, 0);
        apiVersionText = Format(apiVersion);

        var a = Marshal.StringToHGlobalAnsi("XuanYu Engine");
        var e = Marshal.StringToHGlobalAnsi("XuanYu Engine");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)s, (byte*)w };
            var appInfo = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = Pack(0, 0, 1), PEngineName = (byte*)e, EngineVersion = Pack(0, 0, 1), ApiVersion = apiVersion };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            var r = _vk.CreateInstance(&ci, null, out inst);
            _instance = inst;
            return r == Result.Success;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }

    public nint GetInstanceProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetInstanceProcAddr(_instance, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    public bool CreateDevice(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, out Silk.NET.Vulkan.Device dev)
    {
        dev = default;
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = qi, QueueCount = 1, PQueuePriorities = &qp };
        var se = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)se };
            var ci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts };
            return _vk!.CreateDevice(pd, &ci, null, out dev) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(se); }
    }

    public nint GetDeviceProc(Silk.NET.Vulkan.Device dev, string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetDeviceProcAddr(dev, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    public bool CreateSurface(Silk.NET.Vulkan.Instance inst, nint fn, nint hinstance, nint hwnd, out SurfaceKHR surf)
    {
        surf = default;
        var f = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfacePtr>(fn);
        var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hinstance, Hwnd = hwnd };
        return f(inst, &ci, null, out surf) == Result.Success;
    }

    static uint Pack(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;
    static string Format(uint v) => $"{v >> 22}.{(v >> 12) & 0x3ff}.{v & 0xfff}";

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, out SurfaceKHR s);
}
