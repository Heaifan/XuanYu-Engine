using System;
using System.Text;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Core;
using XuanYu.Render.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

// VK4-A：物理设备选择器。在已有 Instance + Surface 前提下枚举并选择可用于渲染/呈现的设备。
// 严禁：创建 VkDevice / VkQueue、建立 Swapchain / ImageView、清屏、Present。
// 可接触 VkPhysicalDevice，但只返回纯数据结果，不把 Vulkan 句柄泄漏给上层（含 UI）。
public sealed unsafe class VulkanPhysicalDeviceSelector
{
    public static VulkanPhysicalDeviceSelection Select(
        Vk vk, Instance instance, SurfaceKHR surface, Action<string>? log = null)
    {
        uint count = 0;
        vk.EnumeratePhysicalDevices(instance, &count, null);
        if (count == 0)
        {
            var msg = "【VulkanDevice】枚举物理设备失败：未找到任何物理设备";
            log?.Invoke(msg); Console.WriteLine(msg);
            return new VulkanPhysicalDeviceSelection(false, null, null, "未枚举到物理设备");
        }
        var devices = stackalloc PhysicalDevice[(int)count];
        vk.EnumeratePhysicalDevices(instance, &count, devices);
        vk.TryGetInstanceExtension(instance, out KhrSurface? khr);
        Log(log, $"【VulkanDevice】开始枚举物理设备；候选数量：{count}");

        VulkanPhysicalDeviceInfo? best = null, info = null;
        VulkanQueueFamilySelection? bestQ = null, q = null;
        string? reason = null;
        for (var i = 0; i < count; i++)
        {
            vk.GetPhysicalDeviceProperties(devices[i], out var props);
            var name = Encoding.UTF8.GetString(props.DeviceName, 256).TrimEnd('\0');
            q = SelectQueueFamilies(vk, khr, devices[i], surface);
            info = new VulkanPhysicalDeviceInfo(name, props.DeviceType, props.ApiVersion,
                props.DeviceType == PhysicalDeviceType.DiscreteGpu, q.HasGraphics && q.HasPresent);
            Log(log, $"【VulkanDevice】候选设备[{i}]：{info.Name}；类型：{TypeName(info.Type)}；" +
                      $"API：{VulkanInstanceLogFormatter.FormatVersion(info.ApiVersion)}；" +
                      $"Graphics 族：{q.GraphicsFamily}；Present 族：{q.PresentFamily}；" +
                      $"Surface 呈现：{(q.HasPresent ? "是" : "否")}；可用性：{(info.IsUsable ? "可用" : "不可用")}");
            if (!info.IsUsable) continue;
            if (best is null || (info.IsDiscrete && !best.IsDiscrete))
            { best = info; bestQ = q; reason = info.IsDiscrete ? "优先独立显卡" : "首个可用设备"; }
        }
        if (best is null || bestQ is null)
        {
            var msg = "【VulkanDevice】未找到可用物理设备：需同时支持 Graphics 队列与 Surface Present";
            log?.Invoke(msg); Console.WriteLine(msg);
            return new VulkanPhysicalDeviceSelection(false, null, null, "无可用设备（缺 Graphics 或 Present）");
        }
        Log(log, $"【VulkanDevice】已选择物理设备：{best.Name}；原因：{reason}；" +
                  $"Graphics 族：{bestQ.GraphicsFamily}；Present 族：{bestQ.PresentFamily}；" +
                  $"队列族合并：{(bestQ.SameFamily ? "是" : "否")}");
        return new VulkanPhysicalDeviceSelection(true, best, bestQ, reason!);
    }
    static VulkanQueueFamilySelection SelectQueueFamilies(
        Vk vk, KhrSurface? khr, PhysicalDevice device, SurfaceKHR surface)
    {
        uint qf = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &qf, null);
        if (qf == 0) return VulkanQueueFamilySelection.None;
        var fam = stackalloc QueueFamilyProperties[(int)qf];
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &qf, fam);
        var g = -1; var p = -1;
        for (var i = 0; i < qf; i++)
        {
            if (g < 0 && fam[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) g = i;
            if (p < 0 && khr is not null)
            {
                Bool32 supported = default;
                khr.GetPhysicalDeviceSurfaceSupport(device, (uint)i, surface, &supported);
                if (supported) p = i;
            }
        }
        var hasG = g >= 0; var hasP = p >= 0;
        return new VulkanQueueFamilySelection(g, p, hasG, hasP, hasG && hasP && g == p);
    }
    static void Log(Action<string>? log, string msg) { log?.Invoke(msg); Console.WriteLine(msg); }

    static string TypeName(PhysicalDeviceType t) => t switch
    {
        PhysicalDeviceType.DiscreteGpu => "独立显卡",
        PhysicalDeviceType.IntegratedGpu => "集成显卡",
        PhysicalDeviceType.VirtualGpu => "虚拟显卡",
        PhysicalDeviceType.Cpu => "软件渲染(CPU)",
        _ => "其他"
    };
}

// VK4-A：物理设备选择结果。Success 为 true 时 Device / Queue 非空。
public sealed record VulkanPhysicalDeviceSelection(
    bool Success,
    VulkanPhysicalDeviceInfo? Device,
    VulkanQueueFamilySelection? Queue,
    string Message);
