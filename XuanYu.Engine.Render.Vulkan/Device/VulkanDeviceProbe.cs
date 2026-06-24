using System.Diagnostics;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Device;

/// <summary>
/// 创建临时 Vulkan Instance，选择支持 Graphics Queue 的物理设备，创建并释放 LogicalDevice。
/// 内部使用 VulkanDeviceInstanceScope 与 VulkanDeviceSelector 完成子任务。
/// </summary>
public static unsafe class VulkanDeviceProbe
{
    public static VulkanDeviceInfo Probe()
    {
        var sw = Stopwatch.StartNew();

        try
        {
            using var scope = new VulkanDeviceInstanceScope();

            var r = scope.CreateInstance();
            if (r != Result.Success)
                return Fail($"Vulkan Device 创建失败：Instance 创建失败：{r}。", sw);

            var selector = new VulkanDeviceSelector();
            var devicesResult = selector.ReadPhysicalDevices(scope.Vk, scope.Instance);
            if (devicesResult.Result != Result.Success)
                return Fail($"Vulkan Device 创建失败：枚举 PhysicalDevice 失败：{devicesResult.Result}。", sw);
            if (devicesResult.PhysicalDevices.Count == 0)
                return Fail("Vulkan Device 创建失败：未找到物理设备。", sw);

            var selected = selector.Select(scope.Vk, devicesResult.PhysicalDevices);
            if (selected is null)
                return Fail("Vulkan Device 创建失败：未找到支持 Graphics Queue 的物理设备。", sw);

            float priority = 1.0f;
            var queueCI = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = (uint)selected.GraphicsQueueFamilyIndex,
                QueueCount = 1,
                PQueuePriorities = &priority
            };
            var deviceCI = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &queueCI
            };

            var device = default(Silk.NET.Vulkan.Device);
            r = scope.Vk.CreateDevice(selected.PhysicalDevice, &deviceCI, null, out device);
            if (r != Result.Success)
            {
                sw.Stop();
                return new VulkanDeviceInfo(VulkanDeviceStatus.Failed,
                    $"Vulkan Device 创建失败：{r}。", selected.Name,
                    selected.DeviceTypeText, selected.GraphicsQueueFamilyIndex, sw.Elapsed.TotalMilliseconds);
            }

            scope.Vk.GetDeviceQueue(device, (uint)selected.GraphicsQueueFamilyIndex, 0, out _);
            scope.Vk.DestroyDevice(device, null);

            sw.Stop();
            return new VulkanDeviceInfo(VulkanDeviceStatus.Created,
                $"创建成功，显卡：{selected.Name}，类型：{selected.DeviceTypeText}，图形队列族：{selected.GraphicsQueueFamilyIndex}。",
                selected.Name, selected.DeviceTypeText, selected.GraphicsQueueFamilyIndex, sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            sw.Stop();
            return Fail($"Vulkan Device 创建失败：{ex.Message}", sw);
        }
    }

    static VulkanDeviceInfo Fail(string msg, Stopwatch sw) =>
        new(VulkanDeviceStatus.Failed, msg, "未知", "未知", -1, sw.Elapsed.TotalMilliseconds);
}
