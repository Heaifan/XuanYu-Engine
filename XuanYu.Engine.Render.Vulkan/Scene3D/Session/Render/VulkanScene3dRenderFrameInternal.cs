using System.Diagnostics;
using FluidWarfare.Render.Camera;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// RenderFrameInternal 主体编排：Fence → Acquire → Reset → Compute → Build → Record → Submit → Present。
/// 成功路径委托到 BuildFrameResult 构造最终结果。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private VulkanScene3dFrameResult RenderFrameInternal(
        VulkanScene3dFrameReason reason,
        SceneCameraPose cameraPose,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws,
        Stopwatch sw)
    {
        if (_vk is null || _swapchainRes is null)
            return FailFrame(reason, "Session 未就绪。");

        _frameIndex++;
        var drawCalls = 0;
        int renderedUnitCount = 0;
        VulkanScene3dCommandRecorder.UnitDrawData[] unitDrawData = [];

        try
        {
            var fence = _swapchainRes.Fence;
            var waitResult = _vk.WaitForFences(_device, 1, ref fence, Vk.True, FrameFenceTimeoutNanoseconds);
            if (waitResult == Result.Timeout)
                return FailFrame(reason, $"GPU Fence 等待超时：{FrameFenceTimeoutNanoseconds / 1_000_000} ms。");
            if (waitResult != Result.Success)
                return FailFrame(reason, $"GPU Fence 等待失败：{waitResult}。");

            var (acquireResult, imgIndex) = AcquireNextImage(reason);
            if (acquireResult is not null) return acquireResult;

            var resetResult = _vk.ResetFences(_device, 1, ref fence);
            if (resetResult != Result.Success)
                return FailFrame(reason, $"GPU Fence 重置失败：{resetResult}。");

            var aspect = _swapchainRes.Extent.Width / (float)_swapchainRes.Extent.Height;
            var vp = ComputeViewProjection(cameraPose, aspect);
            (unitDrawData, renderedUnitCount) = BuildUnitDrawData(vp, unitDraws);
            var cursorData = BuildGroundCursorData(vp);
            var (overlayVtxCount, overlayBuf, overlayPipe, overlayLayout) = BuildOverlay(cameraPose);

            if (!VulkanScene3dCommandRecorder.Record(_vk, _swapchainRes.CommandBuffer,
                    _swapchainRes.RenderPass, _swapchainRes.Framebuffers[imgIndex],
                    _swapchainRes.Extent,
                    _gridPipeline, _unitPipeline, _pipelineLayout,
                    vp, _gridBuffer, _gridVertexCount,
                    _unitBuffer, _unitVertexCount,
                    unitDrawData, cursorData,
                    overlayBuf, overlayVtxCount,
                    overlayPipe, overlayLayout,
                    _swapchainRes.Extent.Width, _swapchainRes.Extent.Height,
                    out drawCalls, out var cmdErr))
                return FailFrame(reason, cmdErr);

            if (SubmitFrame() != Result.Success)
                return FailFrame(reason, "QueueSubmit 失败。");

            var presentRes = PresentFrame(imgIndex);
            var presentResult = ClassifyPresentResult(presentRes, reason);
            if (presentResult is not null) return presentResult;

            sw.Stop();
            return BuildFrameResult(reason, cameraPose, sw,
                renderedUnitCount, drawCalls, presentRes, vp);
        }
        catch (Exception ex)
        {
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                $"帧渲染异常：{ex.Message}");
        }
    }
}
