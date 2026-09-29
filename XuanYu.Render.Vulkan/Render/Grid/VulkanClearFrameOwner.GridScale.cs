using System.Numerics;
using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// GRID-UX-R1：地面网格每帧统一消费 ViewportMetricScale，并保留相邻 1/2/5 级。
public sealed unsafe partial class VulkanClearFrameOwner
{
    ViewportMetricScale _lastViewportMetric = new(1.0, 1.0, 1.0);
    ReferenceGridLevels _referenceGridLevels = ReferenceGridScale.Compute(1.0);

    public void UpdateReferenceGridScale(RenderProjection projection)
    {
        var dpi = projection.ViewportDpiScale;
        var viewport = new ViewportState(
            0, 0, _extent.Width / dpi, _extent.Height / dpi,
            (int)_extent.Width, (int)_extent.Height, dpi, _swapchainOwner.ResourceGeneration);
        const double height = 0.0; // GRID-RW-2A：World Reference Plane 固定 Z=0，不随 MapGround 移动。
        var metricValid = ViewportMetricScale.TryCreate(projection.Camera, viewport, height, out var metric);
        if (metricValid)
        {
            _lastViewportMetric = metric;
            _referenceGridLevels = ReferenceGridScale.Compute(metric);
        }
    }

    // 前 40 float 填充 VP/InvVP/相机/视口；后 8 float 由各辅助 Pass 专用。
    void FillGridPushConstants(float[] scene, RenderProjection projection)
    {
        var camera = projection.Camera;
        var viewport = new ViewportState(
            0, 0, _extent.Width, _extent.Height,
            (int)_extent.Width, (int)_extent.Height, 1, _swapchainOwner.ResourceGeneration);
        var state = camera.ToViewProjection(viewport);
        var vulkanProjection = ToVulkanProjection(state.Projection);
        var viewProjection = state.View * vulkanProjection;
        var inverseFinite = Matrix4x4.Invert(viewProjection, out var inverse);
        var ray = WorldRayFactory.FromViewportPoint(state, _extent.Width * 0.5, _extent.Height * 0.5);
        var planeT = -ray.Origin.Z / ray.Direction.Z;
        TraceGridMath(IsFinite(viewProjection), inverseFinite && IsFinite(inverse),
            double.IsFinite(planeT) && planeT > 0 && planeT <= camera.FarPlane);
        fixed (float* pScene = scene)
        {
            FillMatrixTranspose(pScene, viewProjection);
            FillMatrixTransposeInverse(pScene + 16, viewProjection);
        }
        scene[32] = (float)state.RenderOrigin.X;
        scene[33] = (float)state.RenderOrigin.Y;
        scene[34] = (float)state.RenderOrigin.Z;
        scene[35] = 1.0f;
        scene[36] = _extent.Width;
        scene[37] = _extent.Height;
        scene[38] = (float)camera.FarPlane;
        scene[39] = 0.0f; // Reverse-Z Grid 不使用距离硬截断；保留 PushConstant 布局兼容性。
    }

    static bool IsFinite(Matrix4x4 m) =>
        float.IsFinite(m.M11) && float.IsFinite(m.M12) && float.IsFinite(m.M13) && float.IsFinite(m.M14) &&
        float.IsFinite(m.M21) && float.IsFinite(m.M22) && float.IsFinite(m.M23) && float.IsFinite(m.M24) &&
        float.IsFinite(m.M31) && float.IsFinite(m.M32) && float.IsFinite(m.M33) && float.IsFinite(m.M34) &&
        float.IsFinite(m.M41) && float.IsFinite(m.M42) && float.IsFinite(m.M43) && float.IsFinite(m.M44);
}
