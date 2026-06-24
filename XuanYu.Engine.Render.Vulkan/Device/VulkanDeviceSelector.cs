using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Device;

/// <summary>PhysicalDevice 枚举 + 选择。优先独立 GPU + Graphics Queue。供 VulkanDeviceProbe 内部使用。</summary>
sealed unsafe class VulkanDeviceSelector
{
    public PhysicalDevicesReadResult ReadPhysicalDevices(
        Vk vk, Silk.NET.Vulkan.Instance instance)
    {
        uint count = 0;
        var r = vk.EnumeratePhysicalDevices(instance, ref count, null);
        if (r != Result.Success) return new(r, []);
        if (count == 0) return new(Result.Success, []);

        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* ptr = devices)
        {
            r = vk.EnumeratePhysicalDevices(instance, ref count, ptr);
            return new(r, devices);
        }
    }

    public SelectedPhysicalDevice? Select(
        Vk vk, IReadOnlyList<Silk.NET.Vulkan.PhysicalDevice> devices)
    {
        var candidates = devices
            .Select(d => TryCreateCandidate(vk, d))
            .Where(c => c is not null)
            .Select(c => c!)
            .ToArray();

        return candidates
            .OrderByDescending(c => c.DeviceType == PhysicalDeviceType.DiscreteGpu)
            .FirstOrDefault();
    }

    static SelectedPhysicalDevice? TryCreateCandidate(
        Vk vk, Silk.NET.Vulkan.PhysicalDevice device)
    {
        vk.GetPhysicalDeviceProperties(device, out var props);
        var idx = FindGraphicsQueueFamilyIndex(vk, device);
        return idx < 0 ? null
            : new SelectedPhysicalDevice(device, ReadDeviceName(props),
                props.DeviceType, props.DeviceType.ToString(), idx);
    }

    static int FindGraphicsQueueFamilyIndex(
        Vk vk, Silk.NET.Vulkan.PhysicalDevice device)
    {
        uint count = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(device, ref count, null);
        if (count == 0) return -1;

        var families = new QueueFamilyProperties[count];
        fixed (QueueFamilyProperties* ptr = families)
            vk.GetPhysicalDeviceQueueFamilyProperties(device, ref count, ptr);

        for (var i = 0; i < families.Length; i++)
            if (families[i].QueueCount > 0 &&
                families[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                return i;
        return -1;
    }

    static string ReadDeviceName(PhysicalDeviceProperties props) =>
        Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
}

sealed record PhysicalDevicesReadResult(
    Result Result,
    IReadOnlyList<Silk.NET.Vulkan.PhysicalDevice> PhysicalDevices);

sealed record SelectedPhysicalDevice(
    Silk.NET.Vulkan.PhysicalDevice PhysicalDevice,
    string Name,
    PhysicalDeviceType DeviceType,
    string DeviceTypeText,
    int GraphicsQueueFamilyIndex);
