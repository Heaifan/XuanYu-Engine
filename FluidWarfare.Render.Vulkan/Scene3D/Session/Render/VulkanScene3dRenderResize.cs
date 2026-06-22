using System.Diagnostics;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Resize 编排器：检查 → DeviceWaitIdle → 保存旧资源 → 创建新资源
/// → 事务切换 → 释放旧资源 → 不变量 → 渲染首帧。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    public VulkanScene3dFrameResult Resize(
        uint newWidth, uint newHeight,
        SceneCameraPose cameraPose,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        if (_status != VulkanScene3dSessionStatus.Active)
            return FailFrame(VulkanScene3dFrameReason.Resize,
                $"Session 状态不允许 Resize（当前 {_status}，仅 Active 允许）。");
        if (_rendering)
            return VulkanScene3dFrameResult.Failed(_frameIndex, VulkanScene3dFrameReason.Resize, "渲染进行中，跳过 Resize。");
        if (newWidth == 0 || newHeight == 0)
            return VulkanScene3dFrameResult.Skipped(_frameIndex, VulkanScene3dFrameReason.Resize,
                _swapchainGeneration, _consecutiveAcquireTimeouts, null,
                $"Resize 忽略 0×0 尺寸（{newWidth}x{newHeight}），等待非零尺寸。");

        _recreateRequested = false;
        _status = VulkanScene3dSessionStatus.RecreatingSwapchain;
        var sw = Stopwatch.StartNew();

        if (_swapchainFunctions is null)
            return FailFrame(VulkanScene3dFrameReason.Resize, "SwapchainFunctions 未初始化。");

        var idleResult = _vk!.DeviceWaitIdle(_device);
        if (idleResult != Result.Success)
        {
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex, VulkanScene3dFrameReason.Resize,
                $"DeviceWaitIdle 失败：{idleResult}。");
        }

        var oldResources = _swapchainRes;
        var oldGridPipeline = _gridPipeline;
        var oldUnitPipeline = _unitPipeline;
        var oldGridPipeOk = _gridPipeOk;
        var oldUnitPipeOk = _unitPipeOk;
        var oldOverlayResources = _overlayResources;

        var oldSc = oldResources?.Swapchain ?? default;
        var createResult = VulkanScene3dSwapchainResources.TryCreate(
            _vk, _device, _physicalDevice, _surface,
            newWidth, newHeight, _queueIndex,
            _swapchainFunctions, oldSc);
        if (!createResult.IsSucceeded)
        {
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            sw.Stop();
            return VulkanScene3dFrameResult.Failed(_frameIndex, VulkanScene3dFrameReason.Resize,
                createResult.Message);
        }

        // 事务：创建新 Pipeline/Overlay → 原子切换 → 释放旧资源
        var res = CreateResizeResources(createResult.Resources!, oldResources,
            oldGridPipeline, oldUnitPipeline, oldGridPipeOk, oldUnitPipeOk, oldOverlayResources);
        if (res is not null) return res;

        _status = VulkanScene3dSessionStatus.Active;
        _recreateRequested = false;
        _consecutiveAcquireTimeouts = 0;

        if (!VulkanScene3dSwapchainInvariant.IsActiveValid())
        {
            var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex, VulkanScene3dFrameReason.Resize,
                $"[严重]Resize 后不变量失败。\n{diag}");
        }

        sw.Stop();
        return RenderFrameInternal(VulkanScene3dFrameReason.Resize, cameraPose, unitDraws, sw);
    }
}
