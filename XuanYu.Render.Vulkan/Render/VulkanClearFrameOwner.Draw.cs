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
        var scene = new float[24];
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
                FillScenePushConstants(pScene, _renderProjection, entities[i].Position);
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
        FillScenePushConstants(scene, projection, projection.GizmoPosition);
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

    void FillScenePushConstants(float* target, RenderProjection projection, Vector3d position)
    {
        var viewport = new ViewportState(
            0,
            0,
            _extent.Width,
            _extent.Height,
            (int)_extent.Width,
            (int)_extent.Height,
            1,
            _swapchainOwner.ResourceGeneration);
        var camera = projection.Camera with { Revision = _swapchainOwner.ResourceGeneration };
        var state = camera.ToViewProjection(viewport);
        var vulkanProjection = ToVulkanProjection(state.Projection);
        var viewProjection = state.View * vulkanProjection;
        FillMatrixTranspose(target, viewProjection);
        target[16] = (float)position.X;
        target[17] = (float)position.Y;
        target[18] = (float)position.Z;
        target[19] = 1.0f;
        target[20] = projection.RotateGizmoVisible ? 1.0f : 0.0f;
    }

}
