using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// Scale Gizmo 屏幕空间恒定尺寸换算（与 RotateGizmoScreenRadius 同思路）。
// CPU 命中布局与 Vulkan 绘制共用同一世界轴长，保证“所看到 ≈ 点得到”。
public static class ScaleGizmoScreenSize
{
    public const double TargetScreenAxisDip = 63.0;
    public const double HandleScreenSizeDip = 8.0;
    public const double CenterScreenSizeDip = 15.0;
    public const double CenterHitRadiusDip = 12.0;

    public static double ComputeWorldAxisLength(
        CameraState camera, ViewportState viewport, Vector3d origin)
    {
        var h = viewport.LogicalHeight;
        if (h <= 0) return 1.2;
        var to = origin - camera.Position;
        var depth = to.Dot(camera.Forward);
        if (depth <= 1e-3) depth = to.Length;
        if (depth <= 1e-3) return 1.2;
        var fov = camera.VerticalFovDegrees * (System.Math.PI / 180.0);
        // F3-F4：正交投影下世界高度恒定等于正交尺度，不随深度变化。
        var worldHeight = camera.Mode == ProjectionMode.Orthographic
            ? camera.OrthographicScale
            : 2.0 * depth * System.Math.Tan(fov / 2.0);
        return TargetScreenAxisDip * worldHeight / h;
    }
}
