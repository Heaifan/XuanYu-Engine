using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

internal sealed class CameraController
{
    CameraState _camera = DefaultEditorCamera.Create(1);
    Vector3d _observationCenter = DefaultEditorCamera.Target;
    double _aspect = 16.0 / 9.0, _dpiScale = 1.0;
    long _revision = 1, _sessionRevision;
    bool _framed;
    CameraSessionSnapshot? _session;

    public CameraState Camera => _camera;
    public Vector3d ObservationCenter => _observationCenter;
    public double Aspect => _aspect;
    public double DpiScale => _dpiScale;
    public long Revision => _revision;
    public bool IsFramed => _framed;
    public CameraSessionSnapshot? Session => _session;

    public long NextRevision() => ++_revision;
    public long NextRevisionAfter(long revision) => _revision = System.Math.Max(_revision, revision) + 1;
    public void AcceptRevision(long revision) => _revision = System.Math.Max(_revision, revision);
    public void SetCamera(CameraState camera) { _camera = camera; AcceptRevision(camera.Revision); }
    public void ApplyFrame(CameraFrameResult frame)
    {
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        AcceptRevision(frame.Camera.Revision);
    }

    public bool SetDpiScale(double value)
    {
        if (!double.IsFinite(value) || value <= 0 || System.Math.Abs(_dpiScale - value) < 0.001) return false;
        _dpiScale = value;
        return true;
    }

    public void SetAspect(double value) => _aspect = value;
    public void MarkFramed() => _framed = true;

    public bool TryBeginNavigation(long pointerId, double x, double y, CameraSessionMode mode,
        int width, int height, OrbitPivotResolution? orbit)
    {
        if (_session is not null || width <= 0 || height <= 0 || x < 0 || y < 0 || x > width || y > height) return false;
        FrozenOrbitSession? frozen = null;
        if (mode == CameraSessionMode.Orbit &&
            (orbit is null || !FrozenOrbitSession.TryBegin(_camera, orbit.Pivot, out frozen))) return false;
        _session = new(++_sessionRevision, pointerId, mode, x, y, _camera, _observationCenter,
            width, height, frozen);
        return true;
    }

    public bool TryPreviewNavigation(long pointerId, double x, double y,
        out CameraFrameResult result, out double dx, out double dy)
    {
        result = default; dx = dy = 0;
        if (_session is not { } session || session.PointerId != pointerId) return false;
        dx = x - session.StartX; dy = y - session.StartY;
        var ok = session.Mode == CameraSessionMode.Orbit
            ? session.FrozenOrbit!.TryMoveTo(dx * 0.008, dy * 0.006, _revision + 1, out result, out _)
            : CameraNavigation.TryPan(session.StartCamera, session.StartCenter, dx, dy,
                session.Height, _revision + 1, out result, out _);
        if (ok) AcceptRevision(result.Camera.Revision);
        return ok;
    }

    public bool TryEndNavigation(long pointerId, out CameraSessionSnapshot session)
    {
        session = null!;
        if (_session is not { } current || current.PointerId != pointerId) return false;
        session = current; _session = null;
        return true;
    }

    public bool TryCancelNavigation(out CameraSessionSnapshot session, out CameraFrameResult restore)
    {
        session = _session!; restore = default;
        if (session is null) return false;
        _session = null; restore = new(session.StartCamera, session.StartCenter);
        return true;
    }
}
