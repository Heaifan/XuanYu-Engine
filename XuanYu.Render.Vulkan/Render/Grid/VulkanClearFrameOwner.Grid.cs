using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R1-D5-R1-F2-R2：参考网格绘制。
// PushConstant 192B（48 float）：
//   mat4 viewProjection @0    mat4 inverseViewProjection @64
//   vec4 cameraPosition @128  vec4 viewportAndFar @144 (xy=视口, z=Far, w=GridMaxDist)
//   vec4 gridScale @160 (x=FineSpacing, y=CoarseSpacing, z=FineWeight, w=CoarseWeight)
//   vec4 mapBounds @176 (x=半宽, y=半深, z=BaseHeight, w=边缘淡出宽度；无地图=0；D3)
public sealed unsafe partial class VulkanClearFrameOwner
{
    const uint GridPushFloatCount = 48;
    public const uint ReferenceGridPushSize = GridPushFloatCount * 4;

    public void SetReferenceGridPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _gridPipeline = pipeline;
        _gridPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views)) throw new InvalidOperationException("Pipeline 注入后 CommandBuffer 重录失败");
    }

    void DrawReferenceGrid(CommandBuffer cb)
    {
        if (_gridPipeline.Handle == 0 || _gridPipelineLayout.Handle == 0) return;
        var scene = new float[GridPushFloatCount];
        FillGridPushConstants(scene, _renderProjection);
        // gridScale：每帧全局尺度（1/2/5 序列 + 互补权重），禁止逐 Fragment LOD。
        var levels = ReferenceGridScale.Compute(_lastViewportMetric.MetersPerDip);
        scene[40] = (float)levels.FineSpacing;
        scene[41] = (float)levels.CoarseSpacing;
        scene[42] = (float)levels.FineWeight;
        scene[43] = (float)levels.CoarseWeight;
        PushGridConstants(cb, scene);
        _vk.CmdDraw(cb, RenderDrawPlan.ReferenceGridVertexCount, 1, 0, 0);
    }

    void PushGridConstants(CommandBuffer cb, float[] scene)
    {
        fixed (float* pScene = scene)
        {
            var range = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                Offset = 0,
                Size = GridPushFloatCount * 4
            };
            _vk.CmdPushConstants(cb, _gridPipelineLayout, range.StageFlags, 0,
                GridPushFloatCount * 4, pScene);
        }
    }
}
