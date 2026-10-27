using System.Text;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

public sealed unsafe partial class VulkanPhysicalDeviceSelector
{
    public static VulkanPhysicalDeviceSelection SelectOffscreen(
        Vk vk, Instance instance, Action<string>? log = null)
    {
        if (!VulkanPhysicalDeviceQuery.TryEnumerate(vk, instance,
                out var devices, out var enumerationError))
        {
            log?.Invoke($"【VulkanDevice】离屏枚举物理设备失败：{enumerationError}");
            return new VulkanPhysicalDeviceSelection(false, default, null, null, enumerationError);
        }
        VulkanPhysicalDeviceInfo? best = null;
        VulkanQueueFamilySelection? bestQueue = null;
        PhysicalDevice bestDevice = default;
        for (var index = 0; index < devices.Length; index++)
        {
            vk.GetPhysicalDeviceProperties(devices[index], out var properties);
            var queue = VulkanQueueFamilySelector.Select(vk, devices[index], null, default,
                requirePresent: false, out _);
            if (!queue.HasGraphics) continue;
            var name = Encoding.UTF8.GetString(properties.DeviceName, 256).TrimEnd('\0');
            var info = new VulkanPhysicalDeviceInfo(name, properties.DeviceType,
                properties.ApiVersion, properties.DeviceType == PhysicalDeviceType.DiscreteGpu, true);
            if (best is not null && (!info.IsDiscrete || best.IsDiscrete)) continue;
            best = info; bestDevice = devices[index];
            bestQueue = queue;
        }
        if (best is null || bestQueue is null)
        {
            log?.Invoke("【VulkanDevice】离屏选择失败：无支持 Graphics 的设备");
            return new VulkanPhysicalDeviceSelection(false, default, null, null, "无 Graphics 队列");
        }
        log?.Invoke($"【VulkanDevice】离屏设备选择成功：{best.Name}；仅要求 Graphics，不创建 Surface/Present");
        return new VulkanPhysicalDeviceSelection(true, bestDevice, best, bestQueue, "离屏 Graphics 设备");
    }
}
