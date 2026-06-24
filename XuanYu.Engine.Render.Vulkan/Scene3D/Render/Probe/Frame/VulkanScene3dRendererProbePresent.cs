using System.Diagnostics;
using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 QueuePresent 与帧结果封装。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static bool ProbePresentFrame(VulkanScene3dRenderResources r, nint fnQueuePresent, uint imgIndex, uint qi)
    {
        var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentPtr>(fnQueuePresent);
        var queue = default(Queue); r.Vk!.GetDeviceQueue(r.Device, qi, 0, out queue);
        var scArr = stackalloc[] { r.Swapchain }; var idxArr = stackalloc[] { imgIndex };
        var sigSem = stackalloc[] { r.SemFin };
        var presentInfo = new PresentInfoKHR { SType = StructureType.PresentInfoKhr, WaitSemaphoreCount = 1, PWaitSemaphores = sigSem, SwapchainCount = 1, PSwapchains = scArr, PImageIndices = idxArr };
        var presentRes = presentFn(queue, &presentInfo);
        return presentRes == Result.Success || presentRes == Result.SuboptimalKhr;
    }

    static VulkanScene3dInfo ProbeBuildFrameResult(VulkanScene3dRenderResources r,
        VulkanCameraInfo camera, Extent2D extent, Format chosenFmt, uint imgCount,
        int gVc, int uVc, ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws,
        int renderedUnitCount, int drawCalls, Stopwatch sw)
    {
        var depthFmtName = VulkanScene3dDepthFormatSelector.FormatName(r.DepthFormat);
        r.Vk!.DeviceWaitIdle(r.Device);
        sw.Stop();
        return new VulkanScene3dInfo(VulkanScene3dStatus.Succeeded,
            $"Vulkan 3D 场景绘制成功：RenderObject {unitDraws.Length}，Unit {renderedUnitCount}，单体顶点 {uVc}，Grid {gVc}，Depth {depthFmtName}，DepthAttachment {r.DepthAttachmentCount}，DrawCall {drawCalls}，用时 {sw.Elapsed.TotalMilliseconds:F2} ms。",
            gVc, gVc / 2, uVc, uVc / 3, unitDraws.Length, renderedUnitCount, 0,
            depthFmtName, r.DepthAttachmentCount, true, drawCalls, (int)extent.Width, (int)extent.Height,
            camera.ToSummary(), sw.Elapsed.TotalMilliseconds);
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result QueuePresentPtr(Queue q, PresentInfoKHR* p);
}
