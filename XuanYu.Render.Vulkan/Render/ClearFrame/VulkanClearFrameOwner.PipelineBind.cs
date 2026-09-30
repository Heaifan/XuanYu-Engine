using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
namespace XuanYu.Render.Vulkan.Render;

// 全屏 Pass 管线绑定分发（网格/轴/原点/导航 Gizmo/视图平面网格/天空）。
public sealed unsafe partial class VulkanClearFrameOwner
{
    void BindFramePipeline(CommandBuffer cb, RenderDrawKind kind)
    {
        switch (ResolveDrawOwner(kind))
        {
            case DrawOwner.EditorBackground:
                if (_skyPipeline.Handle == 0 || _skyPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _skyPipeline);
                return;
            case DrawOwner.Terrain:
                if (_terrainPipeline.Handle == 0 || _terrainPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _terrainPipeline);
                return;
            case DrawOwner.VectorOverlay:
                if (_vectorOverlayPipeline.Handle == 0 || _vectorOverlayPipelineLayout.Handle == 0 ||
                    _vectorStrokePipeline.Handle == 0 || _vectorStrokePipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _vectorOverlayPipeline);
                return;
            case DrawOwner.EditorReferenceGrid:
                if (_gridPipeline.Handle == 0 || _gridPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _gridPipeline);
                return;
            case DrawOwner.WorldOrigin:
                if (_originPipeline.Handle == 0 || _originPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _originPipeline);
                return;
            case DrawOwner.WorldAxes:
                if (_axesPipeline.Handle == 0 || _axesPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _axesPipeline);
                return;
            case DrawOwner.NavigationGizmo:
                if (_navGizmoPipeline.Handle == 0 || _navGizmoPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _navGizmoPipeline);
                return;
            case DrawOwner.ScaleIndicatorOverlay:
                if (_scaleIndicatorPipeline.Handle == 0 || _scaleIndicatorPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _scaleIndicatorPipeline);
                return;
            case DrawOwner.EditorViewPlaneGrid:
                if (_viewPlaneGridPipeline.Handle == 0 || _viewPlaneGridPipelineLayout.Handle == 0) return;
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _viewPlaneGridPipeline);
                return;
            case DrawOwner.MapGround:
            case DrawOwner.MapBounds:
            case DrawOwner.Entity:
            case DrawOwner.Gizmo:
                _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _pipeline);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported RenderDrawKind");
        }
    }
}
