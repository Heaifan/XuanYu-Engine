using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

// F3-F2：失败安全导航入口（partial）——Try* 成功才输出结果；失败给出原因且不修改任何状态。
// 所有产生新相机姿态的路径统一经 TryResult → CameraBasis 正交基生成（唯一路径）。
public static partial class CameraNavigation
{
    const double YawPerPixel = 0.008;
    const double PitchPerPixel = 0.006;
    const double MaxPitch = 1.4835298641951802;
    const double MinDistance = 0.25;
    const double MaxDistance = 1_000_000_000.0;

    public static bool TryDolly(CameraState start, Vector3d center, double wheelDelta, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        if (!double.IsFinite(wheelDelta) || wheelDelta == 0.0) { failureReason = "滚轮增量无效或为零"; return false; }
        var distance = ClampDistance(start.Position.DistanceTo(center));
        var nextDistance = ClampDistance(distance * global::System.Math.Pow(0.85, wheelDelta));
        var position = center - (start.Forward * nextDistance);
        return TryResult(start, position, center, revision, out result, out failureReason);
    }

    public static bool TryOrbit(CameraState start, Vector3d center, double dx, double dy, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
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
        return TryResult(start, center + nextOffset, center, revision, out result, out failureReason);
    }

    public static bool TryPan(CameraState start, Vector3d center, double dx, double dy, int height, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        var distance = ClampDistance(start.Position.DistanceTo(center));
        var scale = PanScale(start.VerticalFovDegrees, distance, height);
        var translation = ((-start.Right * dx) + (start.Up * dy)) * scale;
        return TryResult(start, start.Position + translation, center + translation, revision, out result, out failureReason);
    }

    static CameraFrameResult Result(CameraState start, Vector3d position, Vector3d center, long revision)
    {
        if (!TryResult(start, position, center, revision, out var result, out _)) return new CameraFrameResult(start, center);
        return result;
    }

    static bool TryResult(CameraState start, Vector3d position, Vector3d center, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        // 统一正交基路径：PreferredUp=start.Up（保留当前 Up 语义）；
        // 顶/底视等平行场景由 CameraBasis 自动回退世界轴，不再硬编码 UnitZ 导致 CameraState 抛异常。
        if (!CameraBasis.TryCreate(position, center, start.Up, out var forward, out _, out var up, out failureReason))
        {
            return false;
        }

        var far = global::System.Math.Max(start.FarPlane, position.DistanceTo(center) * 4.0);
        result = new CameraFrameResult(
            new CameraState(position, forward, up,
                start.VerticalFovDegrees, start.NearPlane, far, revision),
            center);
        return true;
    }
}
