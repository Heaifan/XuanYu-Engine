using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public static partial class CameraNavigation
{
    public static bool TryDolly(CameraState start, Vector3d center, double wheelDelta, long revision,
        out CameraFrameResult result, out string failureReason)
        => TryDolly(start, center, wheelDelta, revision, null, out result, out failureReason);

    public static bool TryDolly(CameraState start, Vector3d center, double wheelDelta, long revision,
        Vector3d? anchor, out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        if (!double.IsFinite(wheelDelta) || wheelDelta == 0.0) { failureReason = "滚轮增量无效或为零"; return false; }
        if (anchor is { } worldAnchor &&
            (!double.IsFinite(worldAnchor.X) || !double.IsFinite(worldAnchor.Y) || !double.IsFinite(worldAnchor.Z)))
        {
            failureReason = "滚轮锚点无效";
            return false;
        }
        // F3-F4：正交模式缩放使用 OrthographicScale，不得用距离模拟缩放（用户冻结）。
        if (start.Mode == ProjectionMode.Orthographic)
        {
            var nextScale = ClampOrthoScale(start.OrthographicScale * global::System.Math.Pow(0.85, wheelDelta));
            var translation = anchor is { } orthoAnchor
                ? OrthoAnchorTranslation(start, orthoAnchor, nextScale / start.OrthographicScale)
                : Vector3d.Zero;
            result = new CameraFrameResult(
                new CameraState(start.Position + translation, start.Forward, start.Up, start.VerticalFovDegrees,
                    start.NearPlane, start.FarPlane, revision, ProjectionMode.Orthographic, nextScale),
                center + translation);
            return true;
        }
        if (anchor is { } perspectiveAnchor)
        {
            var factor = global::System.Math.Pow(0.85, wheelDelta);
            var translation = (perspectiveAnchor - start.Position) * (1.0 - factor);
            return TryAnchoredPerspectiveResult(start, center, translation, revision,
                out result, out failureReason);
        }
        var distance = ClampDistance(start.Position.DistanceTo(center));
        var nextDistance = ClampDistance(distance * global::System.Math.Pow(0.85, wheelDelta));
        var position = center - (start.Forward * nextDistance);
        return TryResult(start, position, center, revision, start.Up, out result, out failureReason);
    }

    static Vector3d OrthoAnchorTranslation(CameraState start, Vector3d anchor, double scaleRatio)
    {
        var offset = anchor - start.Position;
        var lateral = (start.Right * offset.Dot(start.Right)) + (start.Up * offset.Dot(start.Up));
        return lateral * (1.0 - scaleRatio);
    }

    static bool TryAnchoredPerspectiveResult(CameraState start, Vector3d center, Vector3d translation,
        long revision, out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        var position = start.Position + translation;
        var far = FarPlaneFor(start.NearPlane, position.DistanceTo(center + translation));
        result = new CameraFrameResult(
            new CameraState(position, start.Forward, start.Up, start.VerticalFovDegrees,
                start.NearPlane, far, revision, ProjectionMode.Perspective),
            center + translation);
        return true;
    }
}
