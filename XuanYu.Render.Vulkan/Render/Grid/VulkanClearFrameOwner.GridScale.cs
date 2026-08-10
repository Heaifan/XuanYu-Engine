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

    public void UpdateReferenceGridScale(RenderProjection projection)
    {
        var dpi = projection.ViewportDpiScale;
        var viewport = new ViewportState(
            0, 0, _extent.Width / dpi, _extent.Height / dpi,
            (int)_extent.Width, (int)_extent.Height, dpi, _swapchainOwner.ResourceGeneration);
        var height = projection.Map.HasMap ? projection.Map.BaseHeightMeters : 0.0;
        if (ViewportMetricScale.TryCreate(projection.Camera, viewport, height, out var metric))
            _lastViewportMetric = metric;
    }

    // 176B PushConstant 前 40 float 填充（VP/InvVP/相机/视口+far）；gridScale 由各 Pass 填写。
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
        // D3：地图平面对齐（Z=BaseHeight）+ 地图矩形边缘淡出；无地图时全零 = 无限 Z=0 网格。
        var map = projection.Map;
        scene[40 + 4] = map.HasMap ? (float)(map.WidthMeters / 2.0) : 0.0f;
        scene[41 + 4] = map.HasMap ? (float)(map.DepthMeters / 2.0) : 0.0f;
        scene[42 + 4] = map.HasMap ? (float)map.BaseHeightMeters : 0.0f;
        scene[43 + 4] = map.HasMap ? (float)(System.Math.Min(map.WidthMeters, map.DepthMeters) * 0.08) : 0.0f;
    }
}
