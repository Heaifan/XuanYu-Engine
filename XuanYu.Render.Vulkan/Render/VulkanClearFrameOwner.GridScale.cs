using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R1-D5-R1-F2-R2：参考网格每帧全局尺度计算（视口中心射线与 Z=0 求交）。
// 求交失败回退：中心 → 视口偏下 60% → 上一帧合法尺度（禁止突然重置为 1）。
public sealed unsafe partial class VulkanClearFrameOwner
{
    double _lastReferenceWorldPerPixel = 1.0;

    public void UpdateReferenceGridScale(RenderProjection projection)
    {
        var viewport = new ViewportState(
            0, 0, _extent.Width, _extent.Height,
            (int)_extent.Width, (int)_extent.Height, 1, _swapchainOwner.ResourceGeneration);
        // F3-F4：正交投影下每像素世界距离为解析式（尺度/视口高）；射线求交在侧视正交下退化。
        if (projection.Camera.Mode == ProjectionMode.Orthographic)
        {
            _lastReferenceWorldPerPixel = projection.Camera.OrthographicScale / viewport.LogicalHeight;
            return;
        }
        var state = projection.Camera.ToViewProjection(viewport);
        var halfW = viewport.LogicalWidth * 0.5;
        var halfH = viewport.LogicalHeight * 0.5;
        if (TrySampleWorldPerPixel(state, halfW, halfH, out var wmpp)) { _lastReferenceWorldPerPixel = wmpp; return; }
        if (TrySampleWorldPerPixel(state, halfW, viewport.LogicalHeight * 0.8, out wmpp)) _lastReferenceWorldPerPixel = wmpp;
    }

    static bool TrySampleWorldPerPixel(ViewProjectionState state, double centerX, double centerY, out double worldPerPixel)
    {
        worldPerPixel = 0.0;
        if (!TryHitZ0(WorldRayFactory.FromViewportPoint(state, centerX, centerY), out var center)) return false;
        if (!TryHitZ0(WorldRayFactory.FromViewportPoint(state, centerX + 1.0, centerY), out var right)) return false;
        if (!TryHitZ0(WorldRayFactory.FromViewportPoint(state, centerX, centerY + 1.0), out var down)) return false;
        worldPerPixel = System.Math.Max(center.DistanceTo(right), center.DistanceTo(down));
        return worldPerPixel > 0.0;
    }

    // 世界射线与 Z=0 平面求交；近似平行或交点在相机后方时失败。
    static bool TryHitZ0(WorldRay ray, out Vector3d hit)
    {
        hit = default;
        if (System.Math.Abs(ray.Direction.Z) < 0.001) return false;
        var t = -ray.Origin.Z / ray.Direction.Z;
        if (t <= 0.0) return false;
        hit = ray.Origin + (ray.Direction * t);
        return true;
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
