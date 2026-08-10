using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
namespace XuanYu.Render.Vulkan.Render;

// 全屏 Pass 管线绑定分发（网格/轴/原点/导航 Gizmo/视图平面网格/天空）。
public sealed unsafe partial class VulkanClearFrameOwner
{
    void BindFramePipeline(CommandBuffer cb, RenderDrawKind kind)
    {
        if (kind == RenderDrawKind.MapVectorOverlay)
        {
            var pipeline = _vectorOverlayPipeline.Handle != 0 ? _vectorOverlayPipeline : _pipeline;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, pipeline); return;
        }
        if (kind == RenderDrawKind.EditorReferenceGrid)
        {
            if (_gridPipeline.Handle == 0 || _gridPipelineLayout.Handle == 0) return;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _gridPipeline); return;
        }
        if (kind == RenderDrawKind.WorldOrigin)
        {
            if (_originPipeline.Handle == 0 || _originPipelineLayout.Handle == 0) return;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _originPipeline); return;
        }
        if (kind == RenderDrawKind.WorldAxes)
        {
            if (_axesPipeline.Handle == 0 || _axesPipelineLayout.Handle == 0) return;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _axesPipeline); return;
        }
        if (kind == RenderDrawKind.NavigationGizmo)
        {
            if (_navGizmoPipeline.Handle == 0 || _navGizmoPipelineLayout.Handle == 0) return;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _navGizmoPipeline); return;
        }
        if (kind == RenderDrawKind.EditorViewPlaneGrid)
        {
            if (_viewPlaneGridPipeline.Handle == 0 || _viewPlaneGridPipelineLayout.Handle == 0) return;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _viewPlaneGridPipeline); return;
        }
        if (kind != RenderDrawKind.EditorBackground)
        {
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _pipeline);
            return;
        }
        if (_skyPipeline.Handle == 0 || _skyPipelineLayout.Handle == 0) return;
        _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _skyPipeline);
    }
}
