using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public sealed class FrozenOrbitSession
{
    const double MaxPitch = 1.4835298641951802;
    readonly CameraState _startCamera;
    readonly Vector3d _pivot;
    readonly double _radius;
    readonly double _startYaw;
    readonly double _startPitch;
    double _yaw;
    double _pitch;

    FrozenOrbitSession(CameraState camera, Vector3d pivot)
    {
        _startCamera = camera;
        _pivot = pivot;
        var offset = camera.Position - pivot;
        _radius = offset.Length;
        _startYaw = global::System.Math.Atan2(offset.Y, offset.X);
        _startPitch = global::System.Math.Asin(offset.Z / _radius);
        _yaw = _startYaw;
        _pitch = _startPitch;
    }

    public Vector3d FrozenPivot => _pivot;
    public double Radius => _radius;

    public static bool TryBegin(CameraState camera, Vector3d pivot, out FrozenOrbitSession? session)
    {
        var offset = camera.Position - pivot;
        if (!double.IsFinite(offset.Length) || offset.Length <= 1e-9)
        {
            session = null;
            return false;
        }

        session = new FrozenOrbitSession(camera, pivot);
        return true;
    }

    public bool TryMove(double yawDelta, double pitchDelta, long revision,
        out CameraFrameResult result, out string failureReason)
        => TryMoveTo(_yaw - _startYaw + yawDelta, _pitch - _startPitch + pitchDelta,
            revision, out result, out failureReason);

    public bool TryMoveTo(double yaw, double pitch, long revision,
        out CameraFrameResult result, out string failureReason)
    {
        result = default; failureReason = "";
        if (!double.IsFinite(yaw) || !double.IsFinite(pitch))
        {
            failureReason = "Orbit 角度无效";
            return false;
        }

        _yaw = _startYaw + yaw;
        _pitch = global::System.Math.Clamp(_startPitch + pitch, -MaxPitch, MaxPitch);
        var horizontal = global::System.Math.Cos(_pitch) * _radius;
        var offset = new Vector3d(global::System.Math.Cos(_yaw) * horizontal,
            global::System.Math.Sin(_yaw) * horizontal,
            global::System.Math.Sin(_pitch) * _radius);
        var position = _pivot + offset;
        if (!CameraBasis.TryCreate(position, _pivot, Vector3d.UnitZ,
            out var forward, out _, out var up, out failureReason)) return false;

        result = new CameraFrameResult(new CameraState(position, forward, up,
            _startCamera.VerticalFovDegrees, _startCamera.NearPlane,
            _startCamera.FarPlane, revision), _pivot);
        return true;
    }
}
