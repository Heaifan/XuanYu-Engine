using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Device;

// VK4-A：物理设备选择结果（纯数据，渲染层）。Success 为 true 时 Handle / Device / Queue 非空。
// Handle 为被选中的 VkPhysicalDevice 原生句柄，供 VK4-B 创建 LogicalDevice 复用；禁止泄漏给 UI。
public sealed record VulkanPhysicalDeviceSelection(
    bool Success,
    PhysicalDevice Handle,
    VulkanPhysicalDeviceInfo? Device,
    VulkanQueueFamilySelection? Queue,
    string Message);
