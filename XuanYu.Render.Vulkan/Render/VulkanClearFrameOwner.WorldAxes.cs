using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R1-D5-R1-F2-R2：世界轴 / 世界原点独立全屏 Pass。
// 单一轴线事实源：网格 Pass 不再画轴；本 Pass 只画 X/Y 世界轴与原点标记。
// 与网格共用 176B PushConstant 布局（gridScale 槽位未使用）。
public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _axesPipeline;
    PipelineLayout _axesPipelineLayout;
    Silk.NET.Vulkan.Pipeline _originPipeline;
    PipelineLayout _originPipelineLayout;

    public void SetWorldAxesPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _axesPipeline = pipeline;
        _axesPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views)) throw new InvalidOperationException("轴 Pass 注入后 CommandBuffer 重录失败");
    }

    public void SetWorldOriginPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _originPipeline = pipeline;
        _originPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views)) throw new InvalidOperationException("原点 Pass 注入后 CommandBuffer 重录失败");
    }

    void DrawWorldAxes(CommandBuffer cb)
    {
        if (_axesPipeline.Handle == 0 || _axesPipelineLayout.Handle == 0) return;
        var scene = new float[VulkanClearFrameOwner.GridPushFloatCount];
        fixed (float* pScene = scene)
        {
            FillGridPushConstants(scene, _renderProjection);
            var range = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                Offset = 0,
                Size = GridPushFloatCount * 4
            };
            _vk.CmdPushConstants(cb, _axesPipelineLayout, range.StageFlags, 0,
                GridPushFloatCount * 4, pScene);
            _vk.CmdDraw(cb, RenderDrawPlan.ReferenceGridVertexCount, 1, 0, 0);
        }
    }

    void DrawWorldOrigin(CommandBuffer cb)
    {
        if (_originPipeline.Handle == 0 || _originPipelineLayout.Handle == 0) return;
        var scene = new float[GridPushFloatCount];
        fixed (float* pScene = scene)
        {
            FillGridPushConstants(scene, _renderProjection);
            var range = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                Offset = 0,
                Size = GridPushFloatCount * 4
            };
            _vk.CmdPushConstants(cb, _originPipelineLayout, range.StageFlags, 0,
                GridPushFloatCount * 4, pScene);
            _vk.CmdDraw(cb, RenderDrawPlan.ReferenceGridVertexCount, 1, 0, 0);
        }
    }
}
