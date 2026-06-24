using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Swapchain;

/// <summary>选择支持 Graphics + Present 的 PhysicalDevice。供 VulkanSwapchainProbe 内部使用。</summary>
sealed unsafe class VulkanSwapchainProbeDeviceSelector
{
    public bool TrySelect(Vk vk, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fnSurfaceSupport,
        out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
    {
        pd = default; qi = 0; name = "未知";
        uint count = 0;
        if (vk.EnumeratePhysicalDevices(inst, ref count, null) != Result.Success || count == 0) return false;

        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices) vk.EnumeratePhysicalDevices(inst, ref count, p);

        if (fnSurfaceSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<SurfaceSupportPtr>(fnSurfaceSupport);

        foreach (var d in devices)
        {
            vk.GetPhysicalDeviceProperties(d, out var props);
            name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps) vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);

            for (uint i = 0; i < qc; i++)
            {
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                {
                    int supported = 0;
                    supportFn(d, i, surf, &supported);
                    if (supported != 0) { pd = d; qi = i; return true; }
                }
            }
        }
        return false;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result SurfaceSupportPtr(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR surf, int* supported);
}
