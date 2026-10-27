using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

internal static unsafe class VulkanPhysicalDeviceQuery
{
    public static bool TryEnumerate(Vk vk, Instance instance,
        out PhysicalDevice[] devices, out string error)
    {
        error = string.Empty;
        uint count = 0;
        var result = vk.EnumeratePhysicalDevices(instance, &count, null);
        if (result != Result.Success)
        {
            devices = [];
            error = $"物理设备数量查询失败：{result}";
            return false;
        }
        if (count == 0)
        {
            devices = [];
            error = "未枚举到物理设备";
            return false;
        }

        devices = new PhysicalDevice[checked((int)count)];
        fixed (PhysicalDevice* pointer = devices)
            result = vk.EnumeratePhysicalDevices(instance, &count, pointer);
        if (result == Result.Success) return true;

        devices = [];
        error = $"物理设备列表查询失败：{result}";
        return false;
    }
}
