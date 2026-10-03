using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

// F3-F2：失败安全导航入口（partial）——Try* 成功才输出结果；失败给出原因且不修改任何状态。
// 所有产生新相机姿态的路径统一经 TryResult → CameraBasis 正交基生成（唯一路径）。
public static partial class CameraNavigation
{
    const double YawPerPixel = 0.008;
    const double PitchPerPixel = 0.006;
    const double MinDistance = 0.25;
    const double MinOrthoScale = 0.001;
    const double MaxOrthoScale = 1_000_000.0;

    public static bool TryOrbit(CameraState start, Vector3d center, double dx, double dy, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        if (!FrozenOrbitSession.TryBegin(start, center, out var session))
        {
            result = default; failureReason = "Orbit Pivot 与相机位置重合";
            return false;
        }

        return session!.TryMoveTo(dx * YawPerPixel, dy * PitchPerPixel, revision,
            out result, out failureReason);
    }

    public static bool TryPan(CameraState start, Vector3d center, double dx, double dy, int height, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        // F3-F4：正交平移按正交尺度换算（每像素世界距离 = 尺度 / 视口高），保持正交模式。
        if (start.Mode == ProjectionMode.Orthographic)
        {
            var orthoScale = start.OrthographicScale / height;
            var orthoTranslation = ((-start.Right * dx) + (start.Up * dy)) * orthoScale;
            return TryResult(start, start.Position + orthoTranslation, center + orthoTranslation, revision, start.Up,
                out result, out failureReason, ProjectionMode.Orthographic, start.OrthographicScale);
        }
        var distance = ClampDistance(start.Position.DistanceTo(center));
        var scale = PanScale(start.VerticalFovDegrees, distance, height);
        var translation = ((-start.Right * dx) + (start.Up * dy)) * scale;
        return TryResult(start, start.Position + translation, center + translation, revision, start.Up,
            out result, out failureReason);
    }

    static bool TryResult(CameraState start, Vector3d position, Vector3d center, long revision,
        Vector3d preferredUp, out CameraFrameResult result, out string failureReason,
        ProjectionMode mode = ProjectionMode.Perspective, double orthographicScale = 0.0)
    {
        result = default; failureReason = "";
        // 统一正交基路径：PreferredUp 由调用方决定（Orbit=世界 +Z 防 Roll；Dolly/Pan=start.Up 保留语义）；
        // 平行场景由 CameraBasis 自动回退世界轴，不再硬编码 UnitZ 导致 CameraState 抛异常。
        if (!CameraBasis.TryCreate(position, center, preferredUp, out var forward, out _, out var up, out failureReason))
        {
            return false;
        }

        var far = FarPlaneFor(start.NearPlane, position.DistanceTo(center));
        result = new CameraFrameResult(
            new CameraState(position, forward, up,
                start.VerticalFovDegrees, start.NearPlane, far, revision, mode, orthographicScale),
            center);
        return true;
    }

    static double ClampOrthoScale(double value) =>
        global::System.Math.Clamp(value, MinOrthoScale, MaxOrthoScale);
}
