using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 Instance / Device / Surface 创建与函数指针加载。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static nint LoadProc(Vk vk, Silk.NET.Vulkan.Instance inst, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetInstanceProcAddr(inst, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }
    static nint LoadDeviceProc(Vk vk, Silk.NET.Vulkan.Device dev, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetDeviceProcAddr(dev, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    static bool CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance inst)
    {
        inst = default;
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare"); var e = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface"); var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try { var exts = stackalloc byte*[] { (byte*)s, (byte*)w }; var ai = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = PackVer(1, 0, 0) }; var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &ai, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts }; return vk.CreateInstance(&ci, null, out inst) == Result.Success; }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }
    static bool CreateSurface(Vk vk, Silk.NET.Vulkan.Instance inst, nint hi, nint hw, out SurfaceKHR s)
    {
        s = default; var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try { var addr = (nint)vk.GetInstanceProcAddr(inst, (byte*)p); if (addr == 0) return false; var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfacePtr>(addr); var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hi, Hwnd = hw }; fixed (SurfaceKHR* sp = &s) return fn(inst, &ci, null, sp) == Result.Success; }
        finally { Marshal.FreeHGlobal(p); }
    }
    static bool SelectDevice(Vk vk, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
    {
        pd = default; qi = 0; name = "未知";
        uint count = 0;
        if (vk.EnumeratePhysicalDevices(inst, ref count, null) != Result.Success || count == 0) return false;
        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices) vk.EnumeratePhysicalDevices(inst, ref count, p);
        var fnSupport = LoadProc(vk, inst, "vkGetPhysicalDeviceSurfaceSupportKHR");
        if (fnSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<SurfaceSupportPtr>(fnSupport);
        foreach (var d in devices)
        {
            vk.GetPhysicalDeviceProperties(d, out var props);
            name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0; vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps) vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);
            for (uint i = 0; i < qc; i++)
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                { int supported = 0; supportFn(d, i, surf, &supported); if (supported != 0) { pd = d; qi = i; return true; } }
        }
        return false;
    }
    static bool CreateDevice(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd, uint qi, out Silk.NET.Vulkan.Device dev)
    {
        dev = default;
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = qi, QueueCount = 1, PQueuePriorities = &qp };
        var se = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try { var exts = stackalloc byte*[] { (byte*)se }; var dci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts }; return vk.CreateDevice(pd, &dci, null, out dev) == Result.Success; }
        finally { Marshal.FreeHGlobal(se); }
    }
    static VulkanScene3dInfo Fail(string msg, Stopwatch sw) =>
        new(VulkanScene3dStatus.Failed, msg, 0, 0, 0, 0, 0, 0, 0, "无", 0, false, 0, 0, 0, "无", sw.Elapsed.TotalMilliseconds);
    static uint PackVer(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result SurfaceSupportPtr(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
}
