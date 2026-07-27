using Silk.NET.Vulkan;
using XuanYu.Core.Math;
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
                // R4-R3-R2：实体主体填充，顶点数=RenderDrawPlan.FillVertexCount(3)
                FillScenePushConstants(pScene, _renderProjection,
                    e.Position, e.Rotation, e.Scale, 0.0f, e.IsSelected ? 1.0f : 0.0f);
                PushSceneConstants(cb, pScene);
                _vk.CmdDraw(cb, RenderDrawPlan.FillVertexCount, 1, 0, 0);
                // R4-R3-R2：选中实体额外绘制外轮廓边带，顶点数=RenderDrawPlan.OutlineRibbonVertexCount(18)
                if (e.IsSelected)
                {
                    FillScenePushConstants(pScene, _renderProjection,
                        e.Position, e.Rotation, e.Scale, 0.0f, 2.0f);
                    PushSceneConstants(cb, pScene);
                    _vk.CmdDraw(cb, RenderDrawPlan.OutlineRibbonVertexCount, 1, 0, 0);
                }
            }
            if (_renderProjection.GizmoVisible || _renderProjection.RotateGizmoVisible || _renderProjection.ScaleGizmoVisible)
                DrawActiveGizmo(cb, pScene, _renderProjection);
        }
    }

    const uint MoveGizmoVertexCount = 39;
    const uint RotateGizmoVertexCount = 867;
    const uint ScaleGizmoVertexCount = 252;

    void DrawActiveGizmo(CommandBuffer cb, float* scene, RenderProjection projection)
    {
        // Scale Gizmo 轴长（worldAxisLength）与实体旋转（GizmoRotation）由 CPU 用 DIP 视口算好并贯穿进来
        // （屏幕空间恒定尺寸，命中层与绘制层共用）；此处直接使用，保证绘制与命中一致。
        Vector3d gizmoRotation;
        double worldRadius;
        uint count;
        if (projection.ScaleGizmoVisible)
        {
            gizmoRotation = projection.GizmoRotation;
            worldRadius = projection.ScaleGizmoWorldRadius;
            count = ScaleGizmoVertexCount;
        }
        else if (projection.RotateGizmoVisible)
        {
            gizmoRotation = Vector3d.Zero;
            worldRadius = projection.RotateGizmoWorldRadius;
            count = RotateGizmoVertexCount;
        }
        else
        {
            gizmoRotation = Vector3d.Zero;
            worldRadius = projection.RotateGizmoWorldRadius;
            count = MoveGizmoVertexCount;
        }
        FillScenePushConstants(scene, projection, projection.GizmoPosition,
            gizmoRotation, new Vector3d(1, 1, 1), (float)worldRadius);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, count, 1, 0, 0);
    }

    void PushSceneConstants(CommandBuffer cb, float* scene)
    {
        _vk.CmdPushConstants(cb, _pipelineLayout,
            ShaderStageFlags.VertexBit, 0,
            VulkanScenePushConstants.SizeInBytes, scene);
    }
}
