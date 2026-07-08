using System;
using Silk.NET.Vulkan;
using VulkanDevice = Silk.NET.Vulkan.Device;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Device;

// VK4-B：LogicalDevice 持有者。基于 VK4-A 的 VulkanPhysicalDeviceSelection 创建 VkDevice 与队列。
// 严禁：重新枚举 PhysicalDevice、自行选择设备、创建 Swapchain / ImageView / RenderPass / CommandBuffer、清屏、Present。
// 队列族索引来自 VK4-A 已确认结果；同族则 Graphics/Present 共用一个队列。
public sealed unsafe class VulkanDeviceOwner : IDisposable
{
    readonly Vk _vk;
    readonly Action<string>? _log;
    VulkanDevice _device;
    Queue _graphicsQueue;
    Queue _presentQueue;
    bool _disposed;

    VulkanDeviceOwner(Vk vk, VulkanDevice device, Queue g, Queue p, Action<string>? log)
    {
        _vk = vk; _device = device; _graphicsQueue = g; _presentQueue = p; _log = log;
    }

    public VulkanDevice LogicalDevice => _device;
    public Queue GraphicsQueue => _graphicsQueue;
    public Queue PresentQueue => _presentQueue;

    public static VulkanDeviceOwner? Create(Vk vk, VulkanPhysicalDeviceSelection sel, string requiredDeviceExtension, Action<string>? log = null)
    {
        if (!sel.Success || sel.Handle.Handle == 0 || sel.Queue is null)
        {
            Log(log, "【VulkanDevice】LogicalDevice 创建跳过：VK4-A 未产出可用物理设备选择");
            return null;
        }
        var q = sel.Queue!;
        Log(log, $"【VulkanDevice】开始创建 LogicalDevice；物理设备：{sel.Device?.Name ?? "未知设备"}");
        Log(log, $"【VulkanDevice】使用的 Graphics 队列族：{q.GraphicsFamily}；Present 队列族：{q.PresentFamily}");
        float priority = 1.0f;
        var gci = new DeviceQueueCreateInfo
        {
            SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = (uint)q.GraphicsFamily,
            QueueCount = 1,
            PQueuePriorities = &priority
        };
        DeviceQueueCreateInfo* pQueues;
        int count;
        if (q.SameFamily)
        {
            pQueues = &gci; count = 1;
        }
        else
        {
            var pci = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = (uint)q.PresentFamily,
                QueueCount = 1,
                PQueuePriorities = &priority
            };
            var arr = stackalloc DeviceQueueCreateInfo[2];
            arr[0] = gci; arr[1] = pci; pQueues = arr; count = 2;
        }
        var dci = new DeviceCreateInfo
        {
            SType = StructureType.DeviceCreateInfo,
            QueueCreateInfoCount = (uint)count,
            PQueueCreateInfos = pQueues
        };
        var extBytes = System.Text.Encoding.ASCII.GetBytes(requiredDeviceExtension); // VK4-C：启用所需设备扩展（当前 VK_KHR_swapchain）才能建 Swapchain
        var extMem = stackalloc byte[extBytes.Length + 1];
        extBytes.CopyTo(new Span<byte>(extMem, extBytes.Length)); extMem[extBytes.Length] = 0;
        byte* pExt = extMem; dci.EnabledExtensionCount = 1; dci.PpEnabledExtensionNames = &pExt;
        var result = vk.CreateDevice(sel.Handle, &dci, null, out var device);
        if (result != Result.Success)
        {
            Log(log, $"【VulkanDevice】LogicalDevice 创建失败：{result}");
            return null;
        }
        Log(log, "【VulkanDevice】LogicalDevice 创建成功");
        vk.GetDeviceQueue(device, (uint)q.GraphicsFamily, 0, out var gq);
        Queue pq = gq;
        if (!q.SameFamily) vk.GetDeviceQueue(device, (uint)q.PresentFamily, 0, out pq);
        Log(log, "【VulkanDevice】Queue 获取成功（Graphics + Present）");
        return new VulkanDeviceOwner(vk, device, gq, pq, log);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_device.Handle != 0) _vk.DestroyDevice(_device, null);
        _device = default; _graphicsQueue = default; _presentQueue = default;
        Log(_log, "【VulkanDevice】LogicalDevice 释放成功");
    }

    static void Log(Action<string>? log, string m) { log?.Invoke(m); Console.WriteLine(m); }
}
