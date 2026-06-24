using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 PhysicalDevice 选择、LogicalDevice 创建与设备级函数指针加载。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static nint LoadDeviceProc(Vk vk, Silk.NET.Vulkan.Device dev, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetDeviceProcAddr(dev, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

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

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result SurfaceSupportPtr(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
}
