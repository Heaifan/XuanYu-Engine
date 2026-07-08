using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

// VK4-A：纯数据物理设备信息。仅描述候选设备，不持有任何 Vulkan 句柄（VkPhysicalDevice 不外露）。
// 该记录驻留于渲染层，UI 仅通过日志字符串间接感知，不直接引用此类型。
public sealed record VulkanPhysicalDeviceInfo(
    string Name,
    PhysicalDeviceType Type,
    uint ApiVersion,
    bool IsDiscrete,
    bool IsUsable);
