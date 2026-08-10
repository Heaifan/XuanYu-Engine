using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// GRID-RW-1：参考网格绘制（GPU procedural world LineList）。
// PushConstant 192B（48 float）：
//   mat4 viewProjection @0    mat4 inverseViewProjection @64
//   vec4 cameraPosition @128  vec4 viewportAndFar @144 (xy=视口, z=Far, w=GridMaxDist)
//   vec4 gridState @160 (x=Step, y=AnchorX, z=AnchorY, w=BaseHeight)
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
        scene[40] = (float)_referenceGridFrameState.StepMeters;
        scene[41] = (float)_referenceGridFrameState.AnchorX;
        scene[42] = (float)_referenceGridFrameState.AnchorY;
        scene[43] = (float)_referenceGridFrameState.BaseHeightMeters;
        PushGridConstants(cb, scene);
        _vk.CmdDraw(cb, RenderDrawPlan.ReferenceGridLineVertexCount, 1, 0, 0);
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
