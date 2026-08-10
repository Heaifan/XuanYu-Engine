using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R3-D2-F1-V2：参考网格每帧统一消费 ViewportMetricScale。
// 求交失败时沿用上一帧合法尺度，禁止突然重置为 1。
public sealed unsafe partial class VulkanClearFrameOwner
{
    ViewportMetricScale _lastViewportMetric = new(1.0, 1.0, 1.0);
    ReferenceGridFrameState _referenceGridFrameState =
        new(ReferenceGridFrameState.MinStepMeters, 0.0, 0.0, 0.0);

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
            _referenceGridFrameState = ReferenceGridFrameState.Create(metric,
                projection.Camera.Position.X, projection.Camera.Position.Y, height, _referenceGridFrameState);
        }
        TraceFarProjection(projection, viewport, metricValid, metric);
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
        fixed (float* pScene = scene)
        {
            FillMatrixTranspose(pScene, viewProjection);
            FillMatrixTransposeInverse(pScene + 16, viewProjection);
        }
        scene[32] = (float)camera.Position.X;
        scene[33] = (float)camera.Position.Y;
        scene[34] = (float)camera.Position.Z;
        scene[35] = 1.0f;
        scene[36] = _extent.Width;
        scene[37] = _extent.Height;
        scene[38] = (float)camera.FarPlane;
        scene[39] = (float)(camera.FarPlane * 0.75); // gridMaxDistance：不满强度到 Far
    }
}
