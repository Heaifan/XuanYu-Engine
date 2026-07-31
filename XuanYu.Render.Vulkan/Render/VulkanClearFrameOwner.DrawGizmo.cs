using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    const uint MoveGizmoVertexCount = 39;
    const uint RotateGizmoVertexCount = 867;
    const uint ScaleGizmoVertexCount = 252;

    void DrawActiveGizmo(CommandBuffer cb, float* scene, RenderProjection projection)
    {
        Vector3d rotation;
        double radius;
        uint count;
        if (projection.ScaleGizmoVisible)
        {
            rotation = projection.GizmoRotation;
            radius = projection.ScaleGizmoWorldRadius;
            count = ScaleGizmoVertexCount;
        }
        else if (projection.RotateGizmoVisible)
        {
            rotation = Vector3d.Zero;
            radius = projection.RotateGizmoWorldRadius;
            count = RotateGizmoVertexCount;
        }
        else
        {
            rotation = Vector3d.Zero;
            radius = projection.RotateGizmoWorldRadius;
            count = MoveGizmoVertexCount;
        }
        FillScenePushConstants(scene, projection, projection.GizmoPosition,
            rotation, new Vector3d(1, 1, 1), (float)radius);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, count, 1, 0, 0);
    }

    void PushSceneConstants(CommandBuffer cb, float* scene)
    {
        _vk.CmdPushConstants(cb, _pipelineLayout, ShaderStageFlags.VertexBit, 0,
            VulkanScenePushConstants.SizeInBytes, scene);
    }
}
