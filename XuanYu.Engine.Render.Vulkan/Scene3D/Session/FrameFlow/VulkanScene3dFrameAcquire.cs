using System.Runtime.InteropServices;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// AcquireNextImageKHR 调用及结果分类。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    /// <summary>
    /// 调用 AcquireNextImageKHR 并分类结果。
    /// 返回 (null, imgIndex) 表示成功可继续；
    /// 返回 (VulkanScene3dFrameResult, 0) 表示需要中断当前帧。
    /// </summary>
    private (VulkanScene3dFrameResult? Result, uint ImageIndex) AcquireNextImage(
        VulkanScene3dFrameReason reason)
    {
        uint imgIndex = 0;
        var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImageFn>(_fnAcquireNextImage);
        var acqRes = acquireFn(_device, _swapchainRes!.Swapchain, AcquireImageTimeoutNanoseconds,
            _swapchainRes.SemAvail, default, &imgIndex);

        // 分类处理 Acquire 结果
        // 关键规则：Acquire 没成功时，不得 Reset Fence。
        var result = ClassifyAcquireResult(acqRes, reason);
        if (result is not null)
            return (result, 0);

        // Success / SuboptimalKhr
        if (acqRes == Result.SuboptimalKhr)
            _recreateRequested = true;
        _consecutiveAcquireTimeouts = 0;
        return (null, imgIndex);
    }

    /// <summary>
    /// 分类处理 AcquireNextImage 结果。
    /// 返回非 null 表示需要中断当前帧（跳过或失败）。
    /// </summary>
    private VulkanScene3dFrameResult? ClassifyAcquireResult(Result acqRes, VulkanScene3dFrameReason reason)
    {
        switch (acqRes)
        {
            case Result.Success:
            case Result.SuboptimalKhr:
                return null;

            case Result.Timeout:
                _consecutiveAcquireTimeouts++;
                if (_consecutiveAcquireTimeouts >= MaxConsecutiveAcquireTimeouts)
                {
                    _status = VulkanScene3dSessionStatus.Failed;
                    return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                        VulkanScene3dFrameStatus.Failed, acqRes, null,
                        _swapchainGeneration, _consecutiveAcquireTimeouts,
                        $"Acquire 连续超时 {MaxConsecutiveAcquireTimeouts} 次，Session 终止。");
                }
                return VulkanScene3dFrameResult.Skipped(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    acqRes, $"Acquire 超时（{_consecutiveAcquireTimeouts}/{MaxConsecutiveAcquireTimeouts}），本帧跳过。");

            case Result.NotReady:
                return VulkanScene3dFrameResult.Skipped(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    acqRes, "Acquire NotReady，本帧跳过。");

            case Result.ErrorOutOfDateKhr:
                _recreateRequested = true;
                return VulkanScene3dFrameResult.RecreateRequested(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    acqRes, "Acquire 返回 OutOfDate，请求重建。");

            case Result.ErrorSurfaceLostKhr:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, acqRes, VulkanScene3dSwapchainStage.SurfaceCapabilities,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Surface 已丢失，Acquire 返回 SurfaceLost。");

            case Result.ErrorDeviceLost:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, acqRes, null,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Device 已丢失，Acquire 返回 DeviceLost。");

            default:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, acqRes, VulkanScene3dSwapchainStage.GetSwapchainImages,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    $"Acquire 未预期错误：{acqRes}。");
        }
    }
}
