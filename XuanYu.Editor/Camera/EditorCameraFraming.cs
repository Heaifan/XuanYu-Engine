using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public static class EditorCameraFraming
{
    const double DefaultFov = 60.0;
    const double Padding = 1.35;
    static readonly Vector3d Direction = (DefaultEditorCamera.Target - DefaultEditorCamera.Position).Normalize();

    public static CameraState FrameAll(IEnumerable<Vector3d> positions, double aspect, long revision)
    {
        return FrameAllWithCenter(positions, aspect, revision).Camera;
    }

    public static CameraFrameResult FrameAllWithCenter(IEnumerable<Vector3d> positions, double aspect, long revision)
    {
        var points = positions.ToArray();
        if (points.Length == 0) return new CameraFrameResult(DefaultEditorCamera.Create(revision), DefaultEditorCamera.Target);
        return Frame(points, aspect, revision, 1.2);
    }

    public static CameraState FrameSelected(Vector3d center, double aspect, long revision) =>
        FrameSelectedWithCenter(center, aspect, revision).Camera;

    public static CameraFrameResult FrameSelectedWithCenter(Vector3d center, double aspect, long revision) =>
        Frame([center], aspect, revision, 1.8);

    static CameraFrameResult Frame(Vector3d[] points, double aspect, long revision, double minRadius)
    {
        var center = Center(points);
        var radius = global::System.Math.Max(minRadius, points.Max(point => point.DistanceTo(center))) * Padding;
        var fov = global::System.Math.Min(DefaultFov, HorizontalFov(DefaultFov, global::System.Math.Max(0.1, aspect)));
        var distance = radius / global::System.Math.Sin(ToRadians(fov) * 0.5);
        var position = center - (Direction * distance);
        var camera = new CameraState(position, Direction, DefaultEditorCamera.Up,
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
