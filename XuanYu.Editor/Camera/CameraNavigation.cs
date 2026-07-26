using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public static class CameraNavigation
{
    const double YawPerPixel = 0.008;
    const double PitchPerPixel = 0.006;
    const double MaxPitch = 1.4835298641951802;
    const double MinDistance = 0.25;
    const double MaxDistance = 1_000_000_000.0;

    public static CameraFrameResult Orbit(CameraState start, Vector3d center, double dx, double dy, long revision)
    {
        var offset = start.Position - center;
        var distance = ClampDistance(offset.Length);
        var yaw = global::System.Math.Atan2(offset.Y, offset.X) + (dx * YawPerPixel);
        var pitch = global::System.Math.Asin(Clamp(offset.Z / distance, -1.0, 1.0)) + (dy * PitchPerPixel);
        pitch = Clamp(pitch, -MaxPitch, MaxPitch);
        var horizontal = global::System.Math.Cos(pitch) * distance;
        var nextOffset = new Vector3d(
            global::System.Math.Cos(yaw) * horizontal,
            global::System.Math.Sin(yaw) * horizontal,
            global::System.Math.Sin(pitch) * distance);
        return Result(start, center + nextOffset, center, revision);
    }

    public static CameraFrameResult Pan(CameraState start, Vector3d center, double dx, double dy, int height, long revision)
    {
        var distance = ClampDistance(start.Position.DistanceTo(center));
        var scale = PanScale(start.VerticalFovDegrees, distance, height);
        var translation = ((-start.Right * dx) + (start.Up * dy)) * scale;
        return Result(start, start.Position + translation, center + translation, revision);
    }

    public static CameraFrameResult Dolly(CameraState start, Vector3d center, double wheelDelta, long revision)
    {
        if (!double.IsFinite(wheelDelta) || wheelDelta == 0.0) return new CameraFrameResult(start, center);
        var distance = ClampDistance(start.Position.DistanceTo(center));
        var nextDistance = ClampDistance(distance * global::System.Math.Pow(0.85, wheelDelta));
        var position = center - (start.Forward * nextDistance);
        return Result(start, position, center, revision);
    }

    static CameraFrameResult Result(CameraState start, Vector3d position, Vector3d center, long revision)
    {
        var forward = (center - position).Normalize();
        var far = global::System.Math.Max(start.FarPlane, position.DistanceTo(center) * 4.0);
        return new CameraFrameResult(
            new CameraState(position, forward, Vector3d.UnitZ,
                start.VerticalFovDegrees, start.NearPlane, far, revision),
            center);
    }

    static double PanScale(double fov, double distance, int height)
    {
        var safeHeight = global::System.Math.Max(1, height);
        var radians = fov * global::System.Math.PI / 180.0;
        return 2.0 * distance * global::System.Math.Tan(radians * 0.5) / safeHeight;
    }

    static double ClampDistance(double value) => Clamp(value, MinDistance, MaxDistance);

    static double Clamp(double value, double min, double max)
    {
        if (!double.IsFinite(value)) return min;
        return global::System.Math.Min(max, global::System.Math.Max(min, value));
    }
}
