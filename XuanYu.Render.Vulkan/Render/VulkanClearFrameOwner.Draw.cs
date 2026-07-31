using Silk.NET.Vulkan;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void RecordDraw(CommandBuffer cb)
    {
        if (_pipeline.Handle == 0 || _pipelineLayout.Handle == 0) return;
        var viewport = new[]
        {
            new Viewport
            {
                X = 0, Y = 0, Width = _extent.Width, Height = _extent.Height,
                MinDepth = 0, MaxDepth = 1
            }
        };
        var scissor = new[]
        {
            new Rect2D
            {
                Offset = new Offset2D { X = 0, Y = 0 },
                Extent = _extent
            }
        };
        var scene = new float[VulkanScenePushConstants.FloatCount];
        fixed (Viewport* pVp = viewport)
        fixed (Rect2D* pSc = scissor)
        fixed (float* pScene = scene)
        {
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _pipeline);
            _vk.CmdSetViewport(cb, 0, 1, pVp);
            _vk.CmdSetScissor(cb, 0, 1, pSc);
            if (!_hasRenderProjection) return;
            foreach (var draw in RenderDrawPlan.GetFrameDrawPlan(_renderProjection))
            {
                if (draw.EntityIndex >= 0) DrawEntity(cb, pScene, draw);
                else DrawGizmo(cb, pScene, draw);
            }
        }
    }

    void DrawEntity(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        var entity = _renderProjection.Entities[draw.EntityIndex];
        var entityMode = draw.EntityType == RenderEntityType.Cube ? -1.0f : -2.0f;
        var selectionMode = draw.Kind == RenderDrawKind.EntityOutline
            ? 2.0f : (entity.IsSelected ? 1.0f : 0.0f);
        FillScenePushConstants(scene, _renderProjection, entity.Position, entity.Rotation,
            entity.Scale, 0.0f, selectionMode, entityMode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }
}
