using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// QueuePresentKHR 调用及结果分类。
/// </summary>
partial class VulkanScene3dSession
{
    private unsafe Result PresentFrame(uint imgIndex)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<QueuePresentFn>(_fnQueuePresent);
        var pws = stackalloc Silk.NET.Vulkan.Semaphore[] { _swapchainRes!.SemFin };
        var psc = stackalloc SwapchainKHR[] { _swapchainRes.Swapchain };
        uint idx = imgIndex;
        var pi = new PresentInfoKHR { SType = StructureType.PresentInfoKhr, WaitSemaphoreCount = 1,
            PWaitSemaphores = pws, SwapchainCount = 1, PSwapchains = psc, PImageIndices = &idx };
        return fn(_queue, &pi);
    }

    /// <summary>
    /// 分类处理 QueuePresent 结果。
    /// 返回非 null 表示需要中断当前帧（致命失败）。
    /// </summary>
    private VulkanScene3dFrameResult? ClassifyPresentResult(Result presentRes, VulkanScene3dFrameReason reason)
    {
        switch (presentRes)
        {
            case Result.Success:
            case Result.SuboptimalKhr:
                if (presentRes == Result.SuboptimalKhr)
                    _recreateRequested = true;
                return null;

            case Result.ErrorOutOfDateKhr:
                _recreateRequested = true;
                return VulkanScene3dFrameResult.RecreateRequested(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    presentRes, "Present 返回 OutOfDate，请求重建。");

            case Result.ErrorSurfaceLostKhr:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, presentRes, VulkanScene3dSwapchainStage.SurfaceCapabilities,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Surface 已丢失，Present 返回 SurfaceLost。");

            case Result.ErrorDeviceLost:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, presentRes, null,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Device 已丢失，Present 返回 DeviceLost。");

            default:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, presentRes, VulkanScene3dSwapchainStage.CreateSwapchain,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    $"Present 未预期错误：{presentRes}。");
        }
    }
}
