using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

public sealed record VulkanDeviceInfo(string Name, PhysicalDeviceType Type, uint ApiVersion);
