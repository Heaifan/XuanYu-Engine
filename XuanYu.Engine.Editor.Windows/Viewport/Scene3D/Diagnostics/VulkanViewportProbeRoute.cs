using XuanYu.Engine.Render.Vulkan.Backend;
using XuanYu.Engine.Render.Vulkan.Instance;
using XuanYu.Engine.Render.Vulkan.Device;
using XuanYu.Engine.Render.Vulkan.Surface;
using XuanYu.Engine.Render.Vulkan.Swapchain;
using XuanYu.Engine.Render.Vulkan.Clear;
using XuanYu.Engine.Render.Vulkan.Scene3D;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.Render.Vulkan.Validation;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;

/// <summary>Vulkan 探测路由。探测方法包含完整逻辑（调用 Probe + 日志 + 状态更新）。
/// 日志通过回调注入，Shell 无需知道探测细节。</summary>
public sealed class VulkanViewportProbeRoute
{
    readonly VulkanViewportProbeState _state = new();
    public VulkanViewportProbeState State => _state;

    public void ProbeBackend(Action<string> info, Action<string> warning)
    {
        _state.Backend = VulkanBackendProbe.Probe();
        if (_state.Backend.IsAvailable) info($"Vulkan 后端状态：{_state.Backend.Message}");
        else warning($"Vulkan 后端不可用：{_state.Backend.Message}");
    }
    public void ProbeValidation(Action<string> info, Action<string> warning)
    {
        _state.Validation = VulkanValidationAvailabilityProbe.Probe();
        if (_state.Validation.IsEnabled) info(_state.Validation.Message);
        else if (_state.Validation.Status != VulkanValidationStatus.Disabled) warning(_state.Validation.Message);
    }
    public void ProbeInstance(Action<string> info, Action<string> warning)
    {
        if (!_state.Backend.IsAvailable)
        { _state.Instance = new(VulkanInstanceStatus.Failed, "Vulkan 后端不可用，跳过 Instance 创建。", "未知", 0, 0); return; }
        _state.Instance = VulkanInstanceProbe.Probe();
        if (_state.Instance.IsCreated) info($"Vulkan Instance 创建成功，API 版本：{_state.Instance.ApiVersionText}，扩展数量：{_state.Instance.ExtensionCount}，用时：{_state.Instance.ElapsedMilliseconds:F2} ms。");
        else warning(_state.Instance.Message);
    }
    public void ProbeDevice(Action<string> info, Action<string> warning)
    {
        if (!_state.Instance.IsCreated)
        { _state.Device = new(VulkanDeviceStatus.Failed, "Vulkan Instance 未创建，跳过 Device 创建。", "未知", "未知", -1, 0); return; }
        _state.Device = VulkanDeviceProbe.Probe();
        if (_state.Device.IsCreated) info($"Vulkan Device 创建成功，显卡：{_state.Device.PhysicalDeviceName}，类型：{_state.Device.PhysicalDeviceTypeText}，队列族：{_state.Device.GraphicsQueueFamilyIndex}，用时：{_state.Device.ElapsedMilliseconds:F2} ms。");
        else warning(_state.Device.Message);
    }
    public void ProbeSurface(VulkanViewportNativeHostInfo host, Action<string> info, Action<string> warning)
    {
        if (!_state.Device.IsCreated)
        { _state.Surface = new(VulkanSurfaceStatus.Failed, "Vulkan Device 未创建，跳过 Surface 创建。", "未知", false, 0); return; }
        if (!host.HasNativeHandle)
        { _state.Surface = new(VulkanSurfaceStatus.Failed, host.Message, host.PlatformText, false, 0); return; }
        _state.Surface = VulkanSurfaceProbe.ProbeWindows(host.InstanceHandle, host.WindowHandle);
        if (_state.Surface.IsCreated) info($"Vulkan Surface 创建成功，平台：{_state.Surface.PlatformText}，用时：{_state.Surface.ElapsedMilliseconds:F2} ms。");
        else if (_state.Surface.Status != VulkanSurfaceStatus.NotChecked) warning(_state.Surface.Message);
    }
    public void ProbeSwapchain(VulkanViewportNativeHostInfo host, uint w, uint h, Action<string> info, Action<string> warning)
    {
        if (!host.HasNativeHandle || host.InstanceHandle == 0 || host.WindowHandle == 0)
        { _state.Swapchain = new(VulkanSwapchainStatus.Failed, "缺少 Windows 原生视口句柄，跳过 Swapchain 创建。", 0, "未知", "未知", 0, 0, 0); return; }
        _state.Swapchain = VulkanSwapchainProbe.ProbeWindows(host.InstanceHandle, host.WindowHandle, w, h);
        if (_state.Swapchain.IsCreated) info($"Vulkan Swapchain 创建成功，图像数量：{_state.Swapchain.ImageCount}，格式：{_state.Swapchain.SurfaceFormatText}，Present：{_state.Swapchain.PresentModeText}，尺寸：{_state.Swapchain.Width}x{_state.Swapchain.Height}，用时：{_state.Swapchain.ElapsedMilliseconds:F2} ms。");
        else if (_state.Swapchain.Status != VulkanSwapchainStatus.NotChecked) warning(_state.Swapchain.Message);
    }
    public void ProbeClear(VulkanViewportNativeHostInfo host, uint w, uint h, string reason, Action<string> info, Action<string> warning)
    {
        if (!host.HasNativeHandle || host.InstanceHandle == 0 || host.WindowHandle == 0)
        { _state.Clear = new(VulkanClearStatus.Failed, "缺少原生句柄，跳过清屏。", "未知", 0, 0, 0); return; }
        _state.Clear = VulkanClearProbe.ProbeWindows(host.InstanceHandle, host.WindowHandle, w, h);
        if (_state.Clear.IsSucceeded) info($"Vulkan 最小清屏成功，颜色：{_state.Clear.ClearColorText}，尺寸：{_state.Clear.Width}x{_state.Clear.Height}，用时：{_state.Clear.ElapsedMilliseconds:F2} ms。");
        else if (_state.Clear.Status != VulkanClearStatus.NotChecked) warning(_state.Clear.Message);
    }
}
