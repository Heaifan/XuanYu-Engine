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
            var entities = _renderProjection.Entities;
            for (var i = 0; i < entities.Count; i++)
            {
                var e = entities[i];
                var isCube = e.EntityType == RenderEntityType.Cube;
                var fillCount = isCube ? RenderDrawPlan.CubeFillVertexCount : RenderDrawPlan.FillVertexCount;
                var outlineCount = isCube ? RenderDrawPlan.CubeOutlineRibbonVertexCount : RenderDrawPlan.OutlineRibbonVertexCount;
                var entityMode = isCube ? -1.0f : 0.0f;
                // R4-R3-R2：实体主体填充，顶点数=RenderDrawPlan.FillVertexCount(3)
                FillScenePushConstants(pScene, _renderProjection,
                    e.Position, e.Rotation, e.Scale, 0.0f,
                    e.IsSelected ? 1.0f : 0.0f, entityMode);
                PushSceneConstants(cb, pScene);
                _vk.CmdDraw(cb, (uint)fillCount, 1, 0, 0);
                // R4-R3-R2：选中实体额外绘制外轮廓边带，顶点数=RenderDrawPlan.OutlineRibbonVertexCount(18)
                if (e.IsSelected)
                {
                    FillScenePushConstants(pScene, _renderProjection,
                        e.Position, e.Rotation, e.Scale, 0.0f, 2.0f, entityMode);
                    PushSceneConstants(cb, pScene);
                    _vk.CmdDraw(cb, (uint)outlineCount, 1, 0, 0);
                }
            }
            if (_renderProjection.GizmoVisible || _renderProjection.RotateGizmoVisible || _renderProjection.ScaleGizmoVisible)
                DrawActiveGizmo(cb, pScene, _renderProjection);
        }
    }

}
