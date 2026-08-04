using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// 旋转环屏幕空间恒定尺寸换算：将目标 DIP 半径按相机深度与视口逻辑高度换算为世界半径。
// CPU 命中（RotateGizmoLayout）与 Shader 绘制（RenderProjection.RotateGizmoWorldRadius）共用同一值，
// 保证“所见即所命中”。纯函数，便于单元测试与生产路径共用同一公式。
public static class RotateGizmoScreenRadius
{
    public const double TargetScreenRadiusDip = 90.0;

    public static double ComputeWorldRadius(
        CameraState camera,
        ViewportState viewport,
        Vector3d origin,
        double targetScreenRadiusDip = TargetScreenRadiusDip)
    {
        var h = viewport.LogicalHeight;
        if (h <= 0) return RotateGizmoLayout.RingRadius;
        var toOrigin = origin - camera.Position;
        var depth = toOrigin.Dot(camera.Forward);
        if (depth <= 1e-3) depth = toOrigin.Length;
        if (depth <= 1e-3) return RotateGizmoLayout.RingRadius;
        var fovRad = camera.VerticalFovDegrees * (global::System.Math.PI / 180.0);
        // F3-F4：正交投影下世界高度恒定等于正交尺度，不随深度变化。
        var worldHeight = camera.Mode == ProjectionMode.Orthographic
            ? camera.OrthographicScale
            : 2.0 * depth * global::System.Math.Tan(fovRad / 2.0);
        return targetScreenRadiusDip * worldHeight / h;
    }
}
