using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public static partial class CameraNavigation
{
    public static CameraFrameResult Orbit(CameraState start, Vector3d center, double dx, double dy, long revision)
    {
        if (!TryOrbit(start, center, dx, dy, revision, out var result, out _)) return new CameraFrameResult(start, center);
        return result;
    }

    public static CameraFrameResult Pan(CameraState start, Vector3d center, double dx, double dy, int height, long revision)
    {
        if (!TryPan(start, center, dx, dy, height, revision, out var result, out _)) return new CameraFrameResult(start, center);
        return result;
    }

    public static CameraFrameResult Dolly(CameraState start, Vector3d center, double wheelDelta, long revision)
    {
        if (!TryDolly(start, center, wheelDelta, revision, out var result, out _)) return new CameraFrameResult(start, center);
        return result;
    }

    static double PanScale(double fov, double distance, int height)
    {
        var safeHeight = global::System.Math.Max(1, height);
        var radians = fov * global::System.Math.PI / 180.0;
        return 2.0 * distance * global::System.Math.Tan(radians * 0.5) / safeHeight;
    }

    static double ClampDistance(double value) => Clamp(value, MinDistance, MaxDistanceMeters);

    static double Clamp(double value, double min, double max)
    {
        if (!double.IsFinite(value)) return min;
        return global::System.Math.Min(max, global::System.Math.Max(min, value));
    }
}
