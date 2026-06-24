using System.Diagnostics;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Resize 事务：创建新 Pipeline/Overlay → 原子切换 → 释放旧资源。
/// 返回 null 成功，VulkanScene3dFrameResult 失败。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private VulkanScene3dFrameResult? CreateResizeResources(
        VulkanScene3dSwapchainResources newResources,
        VulkanScene3dSwapchainResources? oldResources,
        Pipeline oldGridPipeline, Pipeline oldUnitPipeline,
        bool oldGridPipeOk, bool oldUnitPipeOk,
        Overlay.VulkanOverlayResources? oldOverlayResources)
    {
        Pipeline newGridPipeline = default, newUnitPipeline = default;
        bool newGridPipeOk = false, newUnitPipeOk = false;
        Overlay.VulkanOverlayResources? newOverlayResources = null;

        try
        {
            if (!VulkanScene3dPipelines.Create(_vk!, _device,
                    newResources.RenderPass, _pipelineLayout,
                    _vertModule, _fragModule,
                    newResources.Extent.Width, newResources.Extent.Height,
                    out newGridPipeline, out newUnitPipeline, out var pipeErr))
            {
                newResources.Dispose();
                DisposeResources();
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, VulkanScene3dFrameReason.Resize, pipeErr);
            }
            newGridPipeOk = true; newUnitPipeOk = true;
            _pipelineCreateCount++;

            newOverlayResources = Overlay.VulkanOverlayResources.TryCreate(
                _vk!, _device, _physicalDevice,
                newResources.RenderPass,
                newResources.Extent.Width, newResources.Extent.Height,
                out var overlayErr);
            if (newOverlayResources is null)
                Debug.WriteLine($"[Overlay] Resize 后资源重建失败：{overlayErr}。");
        }
        catch
        {
            newOverlayResources?.Dispose();
            newResources.Dispose();
            if (newGridPipeOk) _vk!.DestroyPipeline(_device, newGridPipeline, null);
            if (newUnitPipeOk) _vk!.DestroyPipeline(_device, newUnitPipeline, null);
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            throw;
        }

        // 原子切换
        _swapchainRes = newResources;
        _gridPipeline = newGridPipeline;
        _unitPipeline = newUnitPipeline;
        _overlayResources = newOverlayResources;
        _gridPipeOk = true; _unitPipeOk = true;
        _swapchainGeneration++;
        _lastPresentedOverlaySnapshot = Overlay.PresentedNavigationOverlaySnapshot.Empty;
        _pendingOverlayLayout = null;

        // 释放旧资源
        if (oldGridPipeOk && oldGridPipeline.Handle != 0)
            _vk!.DestroyPipeline(_device, oldGridPipeline, null);
        if (oldUnitPipeOk && oldUnitPipeline.Handle != 0)
            _vk!.DestroyPipeline(_device, oldUnitPipeline, null);
        oldOverlayResources?.Dispose();
        oldResources?.Dispose();

        return null;
    }
}
