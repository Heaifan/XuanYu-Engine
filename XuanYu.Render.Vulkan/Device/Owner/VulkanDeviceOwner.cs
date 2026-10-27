using System;
using Silk.NET.Vulkan;
using VulkanDevice = Silk.NET.Vulkan.Device;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Device;

// VK4-B：LogicalDevice 持有者。基于 VK4-A 的 VulkanPhysicalDeviceSelection 创建 VkDevice 与队列。
// 严禁：重新枚举 PhysicalDevice、自行选择设备、创建 Swapchain / ImageView / RenderPass / CommandBuffer、清屏、Present。
// 队列族索引来自 VK4-A 已确认结果；同族则 Graphics/Present 共用一个队列。
public sealed unsafe partial class VulkanDeviceOwner : IDisposable
{
    readonly Vk _vk;
    readonly Action<string>? _log;
    VulkanDevice _device;
    Queue _graphicsQueue;
    Queue _presentQueue;
    PhysicalDevice _physicalDevice;
    bool _disposed;
    public bool PresentFenceEnabled { get; }

    VulkanDeviceOwner(Vk vk, VulkanDevice device, PhysicalDevice physicalDevice, Queue g, Queue p, Action<string>? log, bool presentFenceEnabled)
    {
        _vk = vk; _device = device; _physicalDevice = physicalDevice; _graphicsQueue = g; _presentQueue = p; _log = log;
        PresentFenceEnabled = presentFenceEnabled;
    }

    public VulkanDevice LogicalDevice => _device;
    public PhysicalDevice PhysicalDevice => _physicalDevice;
    public Queue GraphicsQueue => _graphicsQueue;
    public Queue PresentQueue => _presentQueue;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_device.Handle != 0) _vk.DestroyDevice(_device, null);
        _device = default; _physicalDevice = default; _graphicsQueue = default; _presentQueue = default;
        Log(_log, "【VulkanDevice】LogicalDevice 释放成功");
    }

    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}
