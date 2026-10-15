using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

public sealed unsafe partial class VulkanDeviceOwner
{
    public static VulkanDeviceOwner? Create(Vk vk, VulkanPhysicalDeviceSelection selection,
        string requiredDeviceExtension, Action<string>? log = null) =>
        CreateCore(vk, selection, requiredDeviceExtension, log, offscreen: false);

    public static VulkanDeviceOwner? CreateOffscreen(
        Vk vk, VulkanPhysicalDeviceSelection selection, Action<string>? log = null) =>
        CreateCore(vk, selection, null, log, offscreen: true);

    static VulkanDeviceOwner? CreateCore(Vk vk, VulkanPhysicalDeviceSelection selection,
        string? extension, Action<string>? log, bool offscreen)
    {
        if (!selection.Success || selection.Handle.Handle == 0 || selection.Queue is null)
        {
            Log(log, "【VulkanDevice】LogicalDevice 创建跳过：设备选择不可用");
            return null;
        }
        var queues = selection.Queue;
        Log(log, $"【VulkanDevice】开始创建 LogicalDevice；物理设备：{selection.Device?.Name ?? "未知设备"}");
        Log(log, offscreen
            ? $"【VulkanDevice】使用的 Graphics 队列族：{queues.GraphicsFamily}；Present：不需要"
            : $"【VulkanDevice】使用的 Graphics 队列族：{queues.GraphicsFamily}；Present 队列族：{queues.PresentFamily}");
        float priority = 1.0f;
        var graphics = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = (uint)queues.GraphicsFamily, QueueCount = 1, PQueuePriorities = &priority };
        DeviceQueueCreateInfo* queueInfos = &graphics;
        var queueCount = 1;
        if (queues.HasPresent && !queues.SameFamily)
        {
            var present = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = (uint)queues.PresentFamily, QueueCount = 1, PQueuePriorities = &priority };
            var both = stackalloc DeviceQueueCreateInfo[2];
            both[0] = graphics; both[1] = present; queueInfos = both; queueCount = 2;
        }
        var create = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo,
            QueueCreateInfoCount = (uint)queueCount, PQueueCreateInfos = queueInfos };
        byte* extensionPointer = null;
        byte* extensionName = null;
        if (!string.IsNullOrWhiteSpace(extension))
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(extension);
            byte* name = stackalloc byte[bytes.Length + 1];
            bytes.CopyTo(new Span<byte>(name, bytes.Length)); name[bytes.Length] = 0;
            extensionPointer = name;
            create.EnabledExtensionCount = 1; create.PpEnabledExtensionNames = &extensionPointer;
        }
        var result = vk.CreateDevice(selection.Handle, &create, null, out var device);
        if (result != Result.Success)
        { Log(log, $"【VulkanDevice】LogicalDevice 创建失败：{result}"); return null; }
        vk.GetDeviceQueue(device, (uint)queues.GraphicsFamily, 0, out var graphicsQueue);
        var presentQueue = graphicsQueue;
        if (queues.HasPresent && !queues.SameFamily)
            vk.GetDeviceQueue(device, (uint)queues.PresentFamily, 0, out presentQueue);
        Log(log, offscreen ? "【VulkanDevice】Graphics Queue 获取成功（无 Present Queue）"
            : "【VulkanDevice】Queue 获取成功（Graphics + Present）");
        return new VulkanDeviceOwner(vk, device, selection.Handle, graphicsQueue, presentQueue, log);
    }

}
