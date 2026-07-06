using Silk.NET.Vulkan;

namespace XuanYu.Editor.UI;

public sealed unsafe partial class VulkanClearSession
{
    void PickDevice()
    {
        uint count = 0;
        _vk.EnumeratePhysicalDevices(_instance, &count, null);
        if (count == 0) throw new InvalidOperationException("未找到 Vulkan PhysicalDevice");
        var devices = stackalloc PhysicalDevice[(int)count];
        _vk.EnumeratePhysicalDevices(_instance, &count, devices);
        for (var d = 0; d < count; d++)
        {
            if (TryPickQueue(devices[d], out _queueFamily)) { _physicalDevice = devices[d]; return; }
        }
        throw new InvalidOperationException("未找到支持 Surface Present 的队列族");
    }

    bool TryPickQueue(PhysicalDevice device, out uint family)
    {
        family = 0;
        uint count = 0;
        _vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, null);
        var props = stackalloc QueueFamilyProperties[(int)count];
        _vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, props);
        for (uint i = 0; i < count; i++)
        {
            _khrSurface!.GetPhysicalDeviceSurfaceSupport(device, i, _surface, out var supported);
            if ((props[i].QueueFlags & QueueFlags.GraphicsBit) != 0 && supported) { family = i; return true; }
        }
        return false;
    }

    void CreateDevice()
    {
        var priority = 1f;
        var queue = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = _queueFamily, QueueCount = 1, PQueuePriorities = &priority };
        var extBytes = System.Text.Encoding.UTF8.GetBytes("VK_KHR_swapchain\0");
        fixed (byte* swapchain = extBytes)
        {
            byte** ext = stackalloc byte*[1]; ext[0] = swapchain;
            var create = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &queue, EnabledExtensionCount = 1, PpEnabledExtensionNames = ext };
            Check(_vk.CreateDevice(_physicalDevice, &create, null, out _device), "创建 Vulkan Device 失败");
        }
        _vk.TryGetDeviceExtension(_instance, _device, out _khrSwapchain);
    }
}
