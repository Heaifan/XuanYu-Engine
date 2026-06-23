using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Context;

sealed unsafe class VulkanRenderContextSelector
{
    public bool Select(Vk vk, Silk.NET.Vulkan.Instance inst,
        out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
    {
        pd = default; qi = 0; name = "未知";
        uint count = 0;
        if (vk.EnumeratePhysicalDevices(inst, ref count, null) != Result.Success || count == 0) return false;

        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* ptr = devices) vk.EnumeratePhysicalDevices(inst, ref count, ptr);

        foreach (var d in devices)
        {
            vk.GetPhysicalDeviceProperties(d, out var props);
            name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qPtr = qProps) vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qPtr);
            for (uint i = 0; i < qc; i++)
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                { pd = d; qi = i; name = name; return true; }
        }
        return false;
    }
}
