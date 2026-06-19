using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Surface;
using FluidWarfare.Render.Vulkan.Swapchain;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Render.Vulkan.Validation;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Editor.Windows.Panels.Viewport;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;

/// <summary>Vulkan 探测路由。每个探测方法返回结果，日志由调用层处理。</summary>
public sealed class VulkanViewportProbeRoute
{
    readonly VulkanViewportProbeState _state = new();
    public VulkanViewportProbeState State => _state;

    public ProbeResult ProbeBackend()
    {
        _state.Backend = VulkanBackendProbe.Probe();
        return new ProbeResult(_state.Backend.IsAvailable, _state.Backend.Message);
    }
    public ProbeResult ProbeInstance()
    {
        if (!_state.Backend.IsAvailable)
        { _state.Instance = new VulkanInstanceInfo(VulkanInstanceStatus.Failed, "Vulkan 后端不可用，跳过 Instance 创建。", "未知", 0, 0); return ProbeResult.Failed(_state.Instance.Message); }
        _state.Instance = VulkanInstanceProbe.Probe();
        return new ProbeResult(_state.Instance.IsCreated, _state.Instance.Message);
    }
    public ProbeResult ProbeDevice()
    {
        if (!_state.Instance.IsCreated)
        { _state.Device = new VulkanDeviceInfo(VulkanDeviceStatus.Failed, "Vulkan Instance 未创建，跳过 Device 创建。", "未知", "未知", -1, 0); return ProbeResult.Failed(_state.Device.Message); }
        _state.Device = VulkanDeviceProbe.Probe();
        return new ProbeResult(_state.Device.IsCreated, _state.Device.Message);
    }
    public ProbeResult ProbeSurface(VulkanViewportNativeHostInfo host)
    {
        if (!_state.Device.IsCreated)
        { _state.Surface = new VulkanSurfaceInfo(VulkanSurfaceStatus.Failed, "Vulkan Device 未创建，跳过 Surface 创建。", "未知", false, 0); return ProbeResult.Failed(_state.Surface.Message); }
        if (!host.HasNativeHandle)
        { _state.Surface = new VulkanSurfaceInfo(VulkanSurfaceStatus.Failed, host.Message, host.PlatformText, false, 0); return ProbeResult.Failed(_state.Surface.Message); }
        _state.Surface = VulkanSurfaceProbe.ProbeWindows(host.InstanceHandle, host.WindowHandle);
        return new ProbeResult(_state.Surface.IsCreated, _state.Surface.Message);
    }
    public ProbeResult ProbeSwapchain(VulkanViewportNativeHostInfo host, uint width, uint height)
    {
        if (!host.HasNativeHandle || host.InstanceHandle == 0 || host.WindowHandle == 0)
        { _state.Swapchain = new VulkanSwapchainInfo(VulkanSwapchainStatus.Failed, "缺少 Windows 原生视口句柄，跳过 Swapchain 创建。", 0, "未知", "未知", 0, 0, 0); return ProbeResult.Failed(_state.Swapchain.Message); }
        _state.Swapchain = VulkanSwapchainProbe.ProbeWindows(host.InstanceHandle, host.WindowHandle, width, height);
        return new ProbeResult(_state.Swapchain.IsCreated, _state.Swapchain.Message);
    }
    public ProbeResult ProbeClear(VulkanViewportNativeHostInfo host, uint width, uint height, string reason)
    {
        if (!host.HasNativeHandle || host.InstanceHandle == 0 || host.WindowHandle == 0)
        { _state.Clear = new VulkanClearInfo(VulkanClearStatus.Failed, "缺少原生句柄，跳过清屏。", "未知", 0, 0, 0); return ProbeResult.Failed(_state.Clear.Message); }
        _state.Clear = VulkanClearProbe.ProbeWindows(host.InstanceHandle, host.WindowHandle, width, height);
        return new ProbeResult(_state.Clear.IsSucceeded, _state.Clear.Message);
    }
    public void ProbeValidation()
    {
        _state.Validation = VulkanValidationAvailabilityProbe.Probe();
    }
    public void ProbeScene3d(VulkanViewportNativeHostInfo host, uint width, uint height,
        int renderSeq, VulkanScene3dVertex[] grid, VulkanScene3dVertex[] unitVerts,
        List<VulkanScene3dUnitDrawInfo> unitDraws, VulkanCameraInfo camera)
    {
        _state.Scene3d = VulkanScene3dRenderer.RenderWindows(
            host.InstanceHandle, host.WindowHandle, width, height, camera,
            grid.AsSpan(), unitVerts.AsSpan(), [.. unitDraws]);
    }
}
