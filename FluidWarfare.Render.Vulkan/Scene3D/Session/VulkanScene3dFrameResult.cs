using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 单帧渲染的结构化结果。
/// 携带帧状态、VkResult、失败阶段、Swapchain 代数、连续超时计数等信息。
/// </summary>
public sealed record VulkanScene3dFrameResult(
    bool Success,
    string Message,
    int FrameIndex,
    VulkanScene3dFrameReason Reason,
    VulkanScene3dFrameStatus FrameStatus,
    Result? VulkanResult,
    VulkanScene3dSwapchainStage? FailureStage,
    int SwapchainGeneration,
    int AcquireTimeoutCount,
    int ViewportWidth,
    int ViewportHeight,
    int RenderedUnitCount,
    int DrawCallCount,
    double CpuElapsedMs)
{
    /// <summary>
    /// 创建一个 Presented 帧结果。
    /// </summary>
    public static VulkanScene3dFrameResult Presented(
        int frameIndex, VulkanScene3dFrameReason reason,
        int swapchainGeneration, int acquireTimeoutCount,
        int viewportWidth, int viewportHeight,
        int renderedUnitCount, int drawCallCount,
        double cpuElapsedMs, string detail) =>
        new(true, detail, frameIndex, reason,
            VulkanScene3dFrameStatus.Presented, Result.Success, null,
            swapchainGeneration, acquireTimeoutCount,
            viewportWidth, viewportHeight,
            renderedUnitCount, drawCallCount, cpuElapsedMs);

    /// <summary>
    /// 创建一个跳过的帧结果（Acquire Timeout / NotReady / 零尺寸）。
    /// </summary>
    public static VulkanScene3dFrameResult Skipped(
        int frameIndex, VulkanScene3dFrameReason reason,
        int swapchainGeneration, int acquireTimeoutCount,
        Result? vkResult, string message) =>
        new(false, message, frameIndex, reason,
            VulkanScene3dFrameStatus.Skipped, vkResult, null,
            swapchainGeneration, acquireTimeoutCount,
            0, 0, 0, 0, 0);

    /// <summary>
    /// 创建一个请求重建的帧结果（OutOfDate / Suboptimal）。
    /// </summary>
    public static VulkanScene3dFrameResult RecreateRequested(
        int frameIndex, VulkanScene3dFrameReason reason,
        int swapchainGeneration, int acquireTimeoutCount,
        Result? vkResult, string message) =>
        new(false, message, frameIndex, reason,
            VulkanScene3dFrameStatus.RecreateRequested, vkResult, null,
            swapchainGeneration, acquireTimeoutCount,
            0, 0, 0, 0, 0);

    /// <summary>
    /// 创建一个致命失败的帧结果。
    /// </summary>
    public static VulkanScene3dFrameResult Failed(
        int frameIndex, VulkanScene3dFrameReason reason,
        VulkanScene3dFrameStatus frameStatus,
        Result? vkResult, VulkanScene3dSwapchainStage? stage,
        int swapchainGeneration, int acquireTimeoutCount,
        string message) =>
        new(false, message, frameIndex, reason,
            frameStatus, vkResult, stage,
            swapchainGeneration, acquireTimeoutCount,
            0, 0, 0, 0, 0);

    /// <summary>
    /// 简明失败（向后兼容，自动推断 FrameStatus=Failed）。
    /// </summary>
    public static VulkanScene3dFrameResult Failed(
        int frameIndex, VulkanScene3dFrameReason reason, string message) =>
        new(false, message, frameIndex, reason,
            VulkanScene3dFrameStatus.Failed, null, null,
            0, 0,
            0, 0, 0, 0, 0);
}
