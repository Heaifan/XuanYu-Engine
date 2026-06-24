using System.Diagnostics;
using XuanYu.Engine.Render.Vulkan.Camera;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的帧渲染编排器：Acquire → MVP → Record → Submit → Present → Result。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    internal static VulkanScene3dInfo ProbeRenderFrame(VulkanScene3dRenderResources r,
        VulkanCameraInfo camera, Extent2D extent, Format chosenFmt, uint imgCount,
        int gVc, int uVc, uint qi, nint fnAcquire, nint fnQueuePresent,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws, Stopwatch sw)
    {
        // Acquire
        var acqRes = ProbeAcquireNextImage(r, fnAcquire, out var imgIndex);
        if (acqRes == Result.ErrorOutOfDateKhr) return Fail("Acquire 返回 OutOfDate。", sw);
        if (acqRes != Result.Success && acqRes != Result.SuboptimalKhr) return Fail($"AcquireNextImage 失败：{acqRes}。", sw);

        // MVP
        var vp = ProbeComputeMVP(camera, extent, out _);
        ProbeComputePerObjectMVP(unitDraws, vp, out var unitMvpData, out var renderedUnitCount);

        // Record
        if (!VulkanScene3dCommandRecorder.Record(r.Vk, r.CommandBuffer, r.RenderPass, r.Framebuffers[imgIndex],
                extent, r.GridPipeline, r.UnitPipeline, r.PipelineLayout, vp,
                r.GridBuffer, gVc, r.UnitBuffer, uVc, unitMvpData,
                null, null, 0, null, null, 0u, 0u, out var drawCalls, out var cmdErr))
            return Fail(cmdErr, sw);

        // Submit
        if (!ProbeSubmitFrame(r, qi)) return Fail("QueueSubmit 失败。", sw);

        // Present + Result
        if (!ProbePresentFrame(r, fnQueuePresent, imgIndex, qi)) return Fail($"QueuePresent 失败：未知。", sw);
        return ProbeBuildFrameResult(r, camera, extent, chosenFmt, imgCount, gVc, uVc,
            unitDraws, renderedUnitCount, drawCalls, sw);
    }
}
