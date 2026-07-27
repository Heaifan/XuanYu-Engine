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
                if (e.IsSelected)
                {
                    FillScenePushConstants(pScene, _renderProjection,
                        e.Position, e.Rotation, e.Scale, 0.0f, 1.0f);
                    PushSceneConstants(cb, pScene);
                    _vk.CmdDraw(cb, 3, 1, 0, 0);
                }
                FillScenePushConstants(pScene, _renderProjection,
                    e.Position, e.Rotation, e.Scale, 0.0f, 0.0f);
                PushSceneConstants(cb, pScene);
                _vk.CmdDraw(cb, 3, 1, 0, 0);
            }
            if (_renderProjection.GizmoVisible || _renderProjection.RotateGizmoVisible)
                DrawActiveGizmo(cb, pScene, _renderProjection);
        }
    }

    const uint MoveGizmoVertexCount = 39;
    const uint RotateGizmoVertexCount = 867;

    void DrawActiveGizmo(CommandBuffer cb, float* scene, RenderProjection projection)
    {
        // 旋转 Gizmo 环的世界半径已由 CPU 用 DIP 视口算好并贯穿进来（屏幕空间恒定尺寸）；
        // 此处直接使用，保证绘制层与命中层在同一世界半径下一致。
        FillScenePushConstants(scene, projection, projection.GizmoPosition,
            Vector3d.Zero, new Vector3d(1, 1, 1),
            (float)projection.RotateGizmoWorldRadius);
        PushSceneConstants(cb, scene);
        var count = projection.RotateGizmoVisible ? RotateGizmoVertexCount : MoveGizmoVertexCount;
        _vk.CmdDraw(cb, count, 1, 0, 0);
    }

    void PushSceneConstants(CommandBuffer cb, float* scene)
    {
        _vk.CmdPushConstants(cb, _pipelineLayout,
            ShaderStageFlags.VertexBit, 0,
            VulkanScenePushConstants.SizeInBytes, scene);
    }
}
