using XuanYu.Engine.Render.Vulkan.Backend;
using XuanYu.Engine.Render.Vulkan.Device;
using XuanYu.Engine.Render.Vulkan.Instance;
using XuanYu.Engine.Render.Vulkan.Surface;
using XuanYu.Engine.Render.Vulkan.Swapchain;
using XuanYu.Engine.Render.Vulkan.Clear;
using XuanYu.Engine.Render.Vulkan.Scene3D;
using XuanYu.Engine.Render.Vulkan.Validation;

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
