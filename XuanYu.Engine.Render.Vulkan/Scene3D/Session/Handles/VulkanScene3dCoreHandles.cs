using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Handles;

/// <summary>Session 级核心 Vulkan 句柄集合。Instance / Device / Surface / Queue。</summary>
sealed record VulkanScene3dCoreHandles(
    Vk Vk,
    Silk.NET.Vulkan.Instance Instance,
    SurfaceKHR Surface,
    Silk.NET.Vulkan.Device Device,
    PhysicalDevice PhysicalDevice,
    uint QueueIndex,
    Queue Queue)
{
    public bool IsValid => Vk is not null && Device.Handle != 0;
}
