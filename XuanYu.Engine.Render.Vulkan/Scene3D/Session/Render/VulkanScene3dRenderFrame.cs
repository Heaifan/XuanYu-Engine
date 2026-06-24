using System.Diagnostics;
using XuanYu.Engine.Render.Camera;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>RenderFrame 入口：状态检查 + 防重入 + 编排。</summary>
unsafe partial class VulkanScene3dSession
{
    public VulkanScene3dFrameResult RenderFrame(
        VulkanScene3dFrameReason reason,
        SceneCameraPose cameraPose,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        if (_status != VulkanScene3dSessionStatus.Active)
            return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                $"Session 状态不允许渲染：{_status}");
        if (_rendering)
            return VulkanScene3dFrameResult.Failed(_frameIndex, reason, "渲染进行中，跳过。");

        _rendering = true;
        try
        {
            var sw = Stopwatch.StartNew();
            return RenderFrameInternal(reason, cameraPose, unitDraws, sw);
        }
        finally
        {
            _rendering = false;
        }
    }
}
