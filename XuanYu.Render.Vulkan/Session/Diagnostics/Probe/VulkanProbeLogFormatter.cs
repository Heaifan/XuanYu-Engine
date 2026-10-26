using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

public static class VulkanProbeLogFormatter
{
    public static string FormatVersion(uint version) =>
        $"{version >> 22}.{(version >> 12) & 0x3ff}.{version & 0xfff}";

    public static string FormatDeviceType(PhysicalDeviceType type) => type switch
    {
        PhysicalDeviceType.DiscreteGpu => "离散型 GPU",
        PhysicalDeviceType.IntegratedGpu => "集成型 GPU",
        PhysicalDeviceType.VirtualGpu => "虚拟 GPU",
        PhysicalDeviceType.Cpu => "CPU",
        PhysicalDeviceType.Other => "其他",
        _ => "未知"
    };
}
