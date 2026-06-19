using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Surface;
using FluidWarfare.Render.Vulkan.Swapchain;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Validation;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;

/// <summary>Vulkan 探测和 Scene3D 诊断状态的单一所有者。</summary>
public sealed class VulkanViewportProbeState
{
    public VulkanBackendInfo Backend { get; set; } = VulkanBackendInfo.NotChecked;
    public VulkanInstanceInfo Instance { get; set; } = VulkanInstanceInfo.NotChecked;
    public VulkanDeviceInfo Device { get; set; } = VulkanDeviceInfo.NotChecked;
    public VulkanSurfaceInfo Surface { get; set; } = VulkanSurfaceInfo.NotChecked;
    public VulkanSwapchainInfo Swapchain { get; set; } = VulkanSwapchainInfo.NotChecked;
    public VulkanClearInfo Clear { get; set; } = VulkanClearInfo.NotChecked;
    public VulkanScene3dInfo Scene3d { get; set; } = VulkanScene3dInfo.NotChecked;
    public VulkanValidationInfo Validation { get; set; } = VulkanValidationInfo.Disabled;
    public VulkanScene3dRunGate Gate { get; set; } = VulkanScene3dRunGate.Evaluate();
}
