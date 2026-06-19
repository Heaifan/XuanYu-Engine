using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Camera;

/// <summary>
/// 相机操作结果。Shell 根据此结果决定是否请求 Scene3D Frame。
/// </summary>
public sealed record ViewportCameraResult(
    bool StateChanged,
    bool NeedsFrame,
    VulkanScene3dFrameReason Reason)
{
    public static readonly ViewportCameraResult NoChange = new(false, false, VulkanScene3dFrameReason.CameraPan);

    public static ViewportCameraResult Frame(VulkanScene3dFrameReason reason) =>
        new(true, true, reason);
}
