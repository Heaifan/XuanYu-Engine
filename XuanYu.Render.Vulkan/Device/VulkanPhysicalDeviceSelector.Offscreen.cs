using System.Text;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

public sealed unsafe partial class VulkanPhysicalDeviceSelector
{
    public static VulkanPhysicalDeviceSelection SelectOffscreen(
        Vk vk, Instance instance, Action<string>? log = null)
    {
        uint count = 0;
        vk.EnumeratePhysicalDevices(instance, &count, null);
        if (count == 0)
            return new VulkanPhysicalDeviceSelection(false, default, null, null, "未枚举到物理设备");
        var devices = stackalloc PhysicalDevice[(int)count];
        vk.EnumeratePhysicalDevices(instance, &count, devices);
        VulkanPhysicalDeviceInfo? best = null;
        VulkanQueueFamilySelection? bestQueue = null;
        PhysicalDevice bestDevice = default;
        for (var index = 0; index < count; index++)
        {
            vk.GetPhysicalDeviceProperties(devices[index], out var properties);
            var graphics = FindGraphicsFamily(vk, devices[index]);
            if (graphics < 0) continue;
            var name = Encoding.UTF8.GetString(properties.DeviceName, 256).TrimEnd('\0');
            var info = new VulkanPhysicalDeviceInfo(name, properties.DeviceType,
                properties.ApiVersion, properties.DeviceType == PhysicalDeviceType.DiscreteGpu, true);
            if (best is not null && (!info.IsDiscrete || best.IsDiscrete)) continue;
            best = info; bestDevice = devices[index];
            bestQueue = new VulkanQueueFamilySelection(graphics, graphics, true, false, false);
        }
        if (best is null || bestQueue is null)
        {
            log?.Invoke("【VulkanDevice】离屏选择失败：无支持 Graphics 的设备");
            return new VulkanPhysicalDeviceSelection(false, default, null, null, "无 Graphics 队列");
        }
        log?.Invoke($"【VulkanDevice】离屏设备选择成功：{best.Name}；仅要求 Graphics，不创建 Surface/Present");
        return new VulkanPhysicalDeviceSelection(true, bestDevice, best, bestQueue, "离屏 Graphics 设备");
    }

    static int FindGraphicsFamily(Vk vk, PhysicalDevice device)
    {
        uint count = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, null);
        if (count == 0) return -1;
        var families = stackalloc QueueFamilyProperties[(int)count];
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, families);
        for (var index = 0; index < count; index++)
            if (families[index].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) return index;
        return -1;
    }
}
