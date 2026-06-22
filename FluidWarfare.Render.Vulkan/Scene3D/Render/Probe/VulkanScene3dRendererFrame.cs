using System.Diagnostics;
using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>诊断探针的帧渲染阶段：Acquire → MVP → Record → Submit → Present → Result。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    internal static VulkanScene3dInfo ProbeRenderFrame(VulkanScene3dRenderResources r,
        VulkanCameraInfo camera, Extent2D extent, Format chosenFmt, uint imgCount,
        int gVc, int uVc, uint qi, nint fnAcquire, nint fnQueuePresent,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws, Stopwatch sw)
    {
        var drawCalls = 0; var renderedUnitCount = 0;
        var depthFmtName = VulkanScene3dDepthFormatSelector.FormatName(r.DepthFormat);

        // Acquire
        r.Vk!.WaitForFences(r.Device, 1, ref r.Fence, Vk.True, ulong.MaxValue);
        r.Vk.ResetFences(r.Device, 1, ref r.Fence);
        uint imgIndex = 0;
        var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImagePtr>(fnAcquire);
        var acqRes = acquireFn(r.Device, r.Swapchain, ulong.MaxValue, r.SemAvail, default, &imgIndex);
        if (acqRes == Result.ErrorOutOfDateKhr) return Fail("Acquire 返回 OutOfDate。", sw);
        if (acqRes != Result.Success && acqRes != Result.SuboptimalKhr) return Fail($"AcquireNextImage 失败：{acqRes}。", sw);

        // MVP
        var aspect = extent.Width / (float)extent.Height;
        var vp = VulkanCameraMatrices.ComputeVulkanMVP(camera, aspect);

        // Per-object MVP
        var unitMvpList = new List<float[]>();
        foreach (var draw in unitDraws)
        {
            var trans = VulkanCameraMatrices.CreateTranslation(draw.X, draw.Y, draw.Z);
            var scale = VulkanCameraMatrices.CreateScale(draw.Scale);
            unitMvpList.Add(VulkanCameraMatrices.Mul(vp, VulkanCameraMatrices.Mul(trans, scale)));
            renderedUnitCount++;
        }
        var oldUnitDrawData = unitMvpList.Select(mvp =>
            new VulkanScene3dCommandRecorder.UnitDrawData(mvp, VulkanScene3dPushConstants.NormalTint)).ToArray();

        // Record
        if (!VulkanScene3dCommandRecorder.Record(r.Vk, r.CommandBuffer, r.RenderPass, r.Framebuffers[imgIndex],
                extent, r.GridPipeline, r.UnitPipeline, r.PipelineLayout, vp,
                r.GridBuffer, gVc, r.UnitBuffer, uVc, oldUnitDrawData,
                null, null, 0, null, null, 0u, 0u, out drawCalls, out var cmdErr))
            return Fail(cmdErr, sw);

        // Submit
        var queue = default(Queue); r.Vk.GetDeviceQueue(r.Device, qi, 0, out queue);
        var waitSem = stackalloc[] { r.SemAvail }; var waitStage = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var sigSem = stackalloc[] { r.SemFin }; var cBufs = stackalloc[] { r.CommandBuffer };
        var submitInfo = new SubmitInfo { SType = StructureType.SubmitInfo, WaitSemaphoreCount = 1, PWaitSemaphores = waitSem, PWaitDstStageMask = waitStage, CommandBufferCount = 1, PCommandBuffers = cBufs, SignalSemaphoreCount = 1, PSignalSemaphores = sigSem };
        if (r.Vk.QueueSubmit(queue, 1, &submitInfo, r.Fence) != Result.Success) return Fail("QueueSubmit 失败。", sw);

        // Present
        var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentPtr>(fnQueuePresent);
        var scArr = stackalloc[] { r.Swapchain }; var idxArr = stackalloc[] { imgIndex };
        var presentInfo = new PresentInfoKHR { SType = StructureType.PresentInfoKhr, WaitSemaphoreCount = 1, PWaitSemaphores = sigSem, SwapchainCount = 1, PSwapchains = scArr, PImageIndices = idxArr };
        var presentRes = presentFn(queue, &presentInfo);
        if (presentRes != Result.Success && presentRes != Result.SuboptimalKhr) return Fail($"QueuePresent 失败：{presentRes}。", sw);

        r.Vk.DeviceWaitIdle(r.Device);
        sw.Stop();

        return new VulkanScene3dInfo(VulkanScene3dStatus.Succeeded,
            $"Vulkan 3D 场景绘制成功：RenderObject {unitDraws.Length}，Unit {renderedUnitCount}，单体顶点 {uVc}，Grid {gVc}，Depth {depthFmtName}，DepthAttachment {r.DepthAttachmentCount}，DrawCall {drawCalls}，用时 {sw.Elapsed.TotalMilliseconds:F2} ms。",
            gVc, gVc / 2, uVc, uVc / 3, unitDraws.Length, renderedUnitCount, 0,
            depthFmtName, r.DepthAttachmentCount, true, drawCalls, (int)extent.Width, (int)extent.Height,
            camera.ToSummary(), sw.Elapsed.TotalMilliseconds);
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result AcquireNextImagePtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result QueuePresentPtr(Queue q, PresentInfoKHR* p);
}
