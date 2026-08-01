using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawGizmo(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        Vector3d rotation;
        double radius;
        float mode;
        if (draw.Kind == RenderDrawKind.ScaleGizmo)
        {
            rotation = _renderProjection.GizmoRotation;
            radius = _renderProjection.ScaleGizmoWorldRadius;
            mode = 2.0f;
        }
        else if (draw.Kind == RenderDrawKind.RotateGizmo)
        {
            rotation = Vector3d.Zero;
            radius = _renderProjection.RotateGizmoWorldRadius;
            mode = 1.0f;
        }
        else
        {
            rotation = Vector3d.Zero;
            radius = _renderProjection.MoveGizmoWorldRadius;
            mode = 0.0f;
        }
        FillScenePushConstants(scene, _renderProjection, _renderProjection.GizmoPosition,
            rotation, new Vector3d(1, 1, 1), (float)radius, gizmoModeOverride: mode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }

    void PushSceneConstants(CommandBuffer cb, float* scene)
    {
        _vk.CmdPushConstants(cb, _pipelineLayout, ShaderStageFlags.VertexBit, 0,
            VulkanScenePushConstants.SizeInBytes, scene);
    }
}
