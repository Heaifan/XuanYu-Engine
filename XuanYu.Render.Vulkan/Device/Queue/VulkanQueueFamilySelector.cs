using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Render.Vulkan.Device;

internal static unsafe class VulkanQueueFamilySelector
{
    public static VulkanQueueFamilySelection Select(Vk vk, PhysicalDevice device,
        KhrSurface? surfaceApi, SurfaceKHR surface, bool requirePresent, out string? error)
    {
        error = null;
        uint count = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, null);
        if (count == 0) return VulkanQueueFamilySelection.None;

        var families = stackalloc QueueFamilyProperties[(int)count];
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, families);
        var graphics = -1;
        var present = -1;
        for (var index = 0; index < count; index++)
        {
            if (graphics < 0 && families[index].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                graphics = index;
            if (!requirePresent || surfaceApi is null || present >= 0) continue;

            Bool32 supported = default;
            var result = surfaceApi.GetPhysicalDeviceSurfaceSupport(
                device, (uint)index, surface, &supported);
            if (result != Result.Success)
            {
                error = $"Surface 队列能力查询失败：family={index}，{result}";
                return VulkanQueueFamilySelection.None;
            }
            if (supported) present = index;
        }

        if (!requirePresent)
            return graphics < 0
                ? VulkanQueueFamilySelection.None
                : new VulkanQueueFamilySelection(graphics, graphics, true, false, false);

        var hasGraphics = graphics >= 0;
        var hasPresent = present >= 0;
        return new VulkanQueueFamilySelection(graphics, present, hasGraphics, hasPresent,
            hasGraphics && hasPresent && graphics == present);
    }
}
