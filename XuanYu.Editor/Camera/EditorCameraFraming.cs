using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public static class EditorCameraFraming
{
    const double DefaultFov = 60.0;
    const double Padding = 1.35;
    static readonly Vector3d Direction = (DefaultEditorCamera.Target - DefaultEditorCamera.Position).Normalize();

    // MAP-A-R1-D4-F4：地图取景使用 45° 斜上方俯视，保证看得到地表内部。
    static readonly Vector3d MapPitchDirection = BuildMapPitchDirection(45.0);

    static Vector3d BuildMapPitchDirection(double pitchDegrees)
    {
        var horizontal = new Vector3d(Direction.X, Direction.Y, 0).Normalize();
        var pitch = ToRadians(pitchDegrees);
        return new Vector3d(
            horizontal.X * System.Math.Cos(pitch),
            horizontal.Y * System.Math.Cos(pitch),
            -System.Math.Sin(pitch));
    }

    public static CameraState FrameAll(IEnumerable<Vector3d> positions, double aspect, long revision)
    {
        return FrameAllWithCenter(positions, aspect, revision).Camera;
    }

    public static CameraFrameResult FrameAllWithCenter(IEnumerable<Vector3d> positions, double aspect, long revision)
    {
        var points = positions.ToArray();
        if (points.Length == 0) return new CameraFrameResult(DefaultEditorCamera.Create(revision), DefaultEditorCamera.Target);
        return Frame(points, aspect, revision, 1.2, Direction);
    }

    // MAP-A-R1-D4-F4：地图取景入口，45° 斜上方俯视完整容纳地图。
    // D5-R1：按目标屏幕占用率（约 70%）计算距离，而不是固定 Padding 导致地图过小。
    public static CameraFrameResult FrameMapAllWithCenter(IEnumerable<Vector3d> positions, double aspect, long revision)
    {
        var points = positions.ToArray();
        if (points.Length == 0) return new CameraFrameResult(DefaultEditorCamera.Create(revision), DefaultEditorCamera.Target);
        var center = Center(points);
        var fov = global::System.Math.Min(DefaultFov, HorizontalFov(DefaultFov, global::System.Math.Max(0.1, aspect)));
        // 地图深度（Y 跨度）投影到垂直视线方向：×cos(45°)。目标占屏幕垂直半宽约 70%。
        // 45° 俯视的透视放大效应使地图投影比平面近似更大，×1.55 补偿（扫描实测 d≈2850 时占用率≈69%）。
        var depth = points.Max(p => p.Y) - points.Min(p => p.Y);
        var halfDepthProj = (depth / 2.0) * global::System.Math.Cos(ToRadians(45.0));
        var targetAngle = ToRadians(fov * 0.5) * 0.70;
        var distance = global::System.Math.Max(50.0, halfDepthProj / global::System.Math.Tan(targetAngle) * 1.55);
        var position = center - (MapPitchDirection * distance);
        var camera = new CameraState(position, MapPitchDirection, DefaultEditorCamera.Up,
            DefaultFov, 0.05, global::System.Math.Max(100.0, distance + (depth * 4.0)), revision);
        return new CameraFrameResult(camera, center);
    }

    public static CameraState FrameSelected(Vector3d center, double aspect, long revision) =>
        FrameSelectedWithCenter(center, aspect, revision).Camera;

    public static CameraFrameResult FrameSelectedWithCenter(Vector3d center, double aspect, long revision) =>
        Frame([center], aspect, revision, 1.8, Direction);

    static CameraFrameResult Frame(Vector3d[] points, double aspect, long revision, double minRadius, Vector3d direction)
    {
        var center = Center(points);
        var radius = global::System.Math.Max(minRadius, points.Max(point => point.DistanceTo(center))) * Padding;
        var fov = global::System.Math.Min(DefaultFov, HorizontalFov(DefaultFov, global::System.Math.Max(0.1, aspect)));
        var distance = radius / global::System.Math.Sin(ToRadians(fov) * 0.5);
        var position = center - (direction * distance);
        var camera = new CameraState(position, direction, DefaultEditorCamera.Up,
            DefaultFov, 0.05, global::System.Math.Max(100.0, distance + (radius * 4.0)), revision);
        return new CameraFrameResult(camera, center);
    }

    static Vector3d Center(Vector3d[] points)
    {
        var minX = points.Min(p => p.X); var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y); var maxY = points.Max(p => p.Y);
        var minZ = points.Min(p => p.Z); var maxZ = points.Max(p => p.Z);
        return new Vector3d((minX + maxX) * 0.5, (minY + maxY) * 0.5, (minZ + maxZ) * 0.5);
    }

    static double HorizontalFov(double verticalFov, double aspect) =>
        ToDegrees(2.0 * global::System.Math.Atan(global::System.Math.Tan(ToRadians(verticalFov) * 0.5) * aspect));

    static double ToRadians(double degrees) => degrees * global::System.Math.PI / 180.0;

    static double ToDegrees(double radians) => radians * 180.0 / global::System.Math.PI;
}
