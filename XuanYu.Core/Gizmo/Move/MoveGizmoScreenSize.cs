using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// Move Gizmo 的屏幕恒定尺寸真源。CPU 布局与 Vulkan 绘制共用同一世界轴长。
public static class MoveGizmoScreenSize
{
    public const double TargetScreenAxisDip = 90.0;
    public const double PlaneOffsetDip = 12.0;
    public const double PlaneArmLengthDip = 16.0;
    public const double PlaneHitPaddingDip = 7.0;

    public static double ComputeWorldAxisLength(
        CameraState camera, ViewportState viewport, Vector3d origin)
    {
        var height = viewport.LogicalHeight;
        if (height <= 0) return MoveGizmoLayout.AxisLength;
        var toOrigin = origin - camera.Position;
        var depth = toOrigin.Dot(camera.Forward);
        if (depth <= 1e-3) depth = toOrigin.Length;
        if (depth <= 1e-3) return MoveGizmoLayout.AxisLength;
        var fov = camera.VerticalFovDegrees * (System.Math.PI / 180.0);
        // F3-F4：正交投影下世界高度恒定等于正交尺度，不随深度变化。
        var worldHeight = camera.Mode == ProjectionMode.Orthographic
            ? camera.OrthographicScale
            : 2.0 * depth * System.Math.Tan(fov / 2.0);
        return TargetScreenAxisDip * worldHeight / height;
    }
}
