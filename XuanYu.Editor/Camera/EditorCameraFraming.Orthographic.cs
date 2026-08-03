using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

// F3-F4：正交取景。保持当前正交模式与观察方向，尺度按包围范围适配
// （竖直跨度与水平跨度/宽高比取大者，全部可见）。
public static partial class EditorCameraFraming
{
    public static CameraFrameResult FrameOrthographicWithCenter(
        IEnumerable<Vector3d> positions, Vector3d forward, Vector3d up,
        double aspect, double distance, long revision)
    {
        var points = positions.ToArray();
        if (points.Length == 0)
            return new CameraFrameResult(DefaultEditorCamera.Create(revision), DefaultEditorCamera.Target);
        var center = Center(points);
        var right = forward.Cross(up).Normalize();
        var spanY = MaxAbsDot(points, center, up) * 2.0 * Padding;
        var spanX = MaxAbsDot(points, center, right) * 2.0 * Padding;
        var scale = System.Math.Max(spanY, spanX / System.Math.Max(0.1, aspect));
        var position = center - (forward * System.Math.Max(1.0, distance));
        var camera = new CameraState(position, forward, up, DefaultFov, 0.05,
            System.Math.Max(100.0, distance + (scale * 4.0)), revision,
            ProjectionMode.Orthographic, scale);
        return new CameraFrameResult(camera, center);
    }

    static double MaxAbsDot(Vector3d[] points, Vector3d center, Vector3d axis) =>
        points.Max(p => System.Math.Abs((p - center).Dot(axis)));
}
