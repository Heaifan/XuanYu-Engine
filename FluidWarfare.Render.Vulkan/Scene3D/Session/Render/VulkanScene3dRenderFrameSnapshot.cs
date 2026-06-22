using System.Diagnostics;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Vulkan.Camera;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 帧结果构建：相机快照 + Overlay 快照 + FrameResult 构造。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private VulkanScene3dFrameResult BuildFrameResult(
        VulkanScene3dFrameReason reason,
        SceneCameraPose cameraPose,
        Stopwatch sw,
        int renderedUnitCount,
        int drawCalls,
        Result presentRes,
        float[] vp)
    {
        // 已成功 Present → 发布相机快照供 Picking 使用
        VulkanSceneRayBuilder.TryInvert(vp, out var invVp, out _);
        _lastPresentedSnapshot = new PresentedCameraSnapshot
        {
            CameraPose = cameraPose,
            ViewProjection = vp,
            InverseViewProjection = invVp ?? [],
            ViewportWidth = (int)_swapchainRes!.Extent.Width,
            ViewportHeight = (int)_swapchainRes.Extent.Height,
            FrameIndex = _frameIndex,
            CameraRevision = cameraPose.Revision
        };
        if (_pendingOverlayLayout is not null)
        {
            _lastPresentedOverlaySnapshot = new Overlay.PresentedNavigationOverlaySnapshot
            {
                Layout = _pendingOverlayLayout,
                ViewportWidth = (int)_swapchainRes.Extent.Width,
                ViewportHeight = (int)_swapchainRes.Extent.Height,
                PresentedFrameIndex = _frameIndex,
                CameraRevision = cameraPose.Revision,
                OverlayRevision = _overlayRevision
            };
        }
        else
        {
            _lastPresentedOverlaySnapshot = Overlay.PresentedNavigationOverlaySnapshot.Empty;
        }

        var finalStatus = _recreateRequested
            ? VulkanScene3dFrameStatus.RecreateRequested
            : VulkanScene3dFrameStatus.Presented;

        return new VulkanScene3dFrameResult(true,
            $"Frame #{_frameIndex} | {reason} | Unit {renderedUnitCount} | DrawCall {drawCalls} | {sw.Elapsed.TotalMilliseconds:F2} ms",
            _frameIndex, reason, finalStatus, presentRes, null,
            _swapchainGeneration, _consecutiveAcquireTimeouts,
            (int)_swapchainRes.Extent.Width, (int)_swapchainRes.Extent.Height,
            renderedUnitCount, drawCalls, sw.Elapsed.TotalMilliseconds);
    }
}
