namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 单帧渲染结果。
/// </summary>
public sealed record VulkanScene3dFrameResult(
    bool Success,
    string Message,
    int FrameIndex,
    VulkanScene3dFrameReason Reason,
    int ViewportWidth,
    int ViewportHeight,
    int RenderedUnitCount,
    int DrawCallCount,
    double CpuElapsedMs)
{
    public static VulkanScene3dFrameResult Failed(int frameIndex, VulkanScene3dFrameReason reason, string message) =>
        new(false, message, frameIndex, reason, 0, 0, 0, 0, 0);
}
