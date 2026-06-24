using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Surface;
using FluidWarfare.Render.Vulkan.Swapchain;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Scene3D;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;

/// <summary>诊断文本格式化。不包含 Shell/面板引用。</summary>
public static class Scene3dDiagnosticText
{
    public static string BackendStatus(VulkanBackendInfo i) =>
        i.IsAvailable ? $"Vulkan 后端状态：{i.Message}" : $"Vulkan 后端不可用：{i.Message}";
    public static string InstanceStatus(VulkanInstanceInfo i) =>
        i.IsCreated ? $"创建成功，API 版本：{i.ApiVersionText}，扩展数量：{i.ExtensionCount}，用时：{i.ElapsedMilliseconds:F2} ms" : i.Message;
    public static string DeviceStatus(VulkanDeviceInfo i) =>
        i.IsCreated ? $"创建成功，显卡：{i.PhysicalDeviceName}，类型：{i.PhysicalDeviceTypeText}，队列族：{i.GraphicsQueueFamilyIndex}，用时：{i.ElapsedMilliseconds:F2} ms" : i.Message;
    public static string SurfaceStatus(VulkanSurfaceInfo i) =>
        i.IsCreated ? $"创建成功，平台：{i.PlatformText}，用时：{i.ElapsedMilliseconds:F2} ms" : i.Message;
    public static string SwapchainStatus(VulkanSwapchainInfo i) =>
        i.IsCreated ? $"创建成功，图像：{i.ImageCount}，格式：{i.SurfaceFormatText}，Present：{i.PresentModeText}，尺寸：{i.Width}x{i.Height}，用时：{i.ElapsedMilliseconds:F2} ms" : i.Message;
    public static string ClearStatus(VulkanClearInfo i) =>
        i.IsSucceeded ? $"成功，{i.ClearColorText}，尺寸：{i.Width}x{i.Height}，用时：{i.ElapsedMilliseconds:F2} ms" : i.Message;
    public static string Scene3dStatus(VulkanScene3dInfo i) =>
        i.IsSucceeded ? "成功" : i.Message;
}
