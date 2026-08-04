using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void FillScenePushConstants(float* target, RenderProjection projection, Vector3d position,
        Vector3d rotation, Vector3d scale, float gizmoRingRadius, float selectionMode = 0.0f,
        float? gizmoModeOverride = null)
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
        target[20] = gizmoModeOverride ?? (projection.ScaleGizmoVisible ? 2.0f
            : (projection.RotateGizmoVisible ? 1.0f : 0.0f));
        target[21] = gizmoRingRadius;
        target[22] = selectionMode;
        target[24] = (float)rotation.X;
        target[25] = (float)rotation.Y;
        target[26] = (float)rotation.Z;
        target[27] = _extent.Width;   // entityRotation.w = viewportWidth
        target[28] = (float)scale.X;
        target[29] = (float)scale.Y;
        target[30] = (float)scale.Z;
        target[31] = _extent.Height;  // entityScale.w = viewportHeight
    }
}
