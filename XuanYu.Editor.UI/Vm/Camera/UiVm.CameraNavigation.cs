using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    CameraSessionSnapshot? _cameraSession;
    long _cameraSessionRevision;
    public Vector3d ObservationCenter => _observationCenter;
    public bool IsCameraNavigationActive => _cameraSession is not null;

    public bool BeginCameraNavigation(long pointerId, double x, double y, bool shift, int width, int height)
    {
        if (_cameraSession is not null || _editorState.InteractionSnapshot.HasCapture) return false;
        if (width <= 0 || height <= 0 || x < 0 || y < 0 || x > width || y > height) return false;
        var mode = shift ? CameraSessionMode.Pan : CameraSessionMode.Orbit;
        var orbit = mode == CameraSessionMode.Orbit ? ResolveOrbitPivot() : null;
        FrozenOrbitSession? frozenOrbit = null;
        if (orbit is not null && !FrozenOrbitSession.TryBegin(_camera, orbit.Pivot, out frozenOrbit)) return false;
        _cameraSession = new CameraSessionSnapshot(++_cameraSessionRevision, pointerId, mode, x, y, _camera,
            _observationCenter, width, height, frozenOrbit);
        if (orbit is not null) StartOrbitProbe(_cameraSession.SessionId, orbit);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, "相机会话开始",
            $"模式={mode}；会话={_cameraSession.SessionId}");
        RefreshLogBindings();
        return true;
    }
    public bool PreviewCameraNavigation(long pointerId, double x, double y)
    {
        if (_cameraSession is not { } session || session.PointerId != pointerId) return false;
        var dx = x - session.StartX;
        var dy = y - session.StartY;
        CameraFrameResult result;
        var ok = session.Mode == CameraSessionMode.Orbit
            ? TryPreviewFrozenOrbit(session, dx, dy, out result)
            : CameraNavigation.TryPan(session.StartCamera, session.StartCenter, dx, dy,
                session.Height, _cameraRevision + 1, out result, out _);
        if (!ok) return false;
        _cameraRevision = result.Camera.Revision;
        ApplyCameraResult(result);
        return true;
    }
    public bool EndCameraNavigation(long pointerId)
    {
        if (_cameraSession is not { } session || session.PointerId != pointerId) return false;
        if (session.Mode == CameraSessionMode.Orbit) EndFrozenOrbit();
        _cameraSession = null;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, "相机会话结束",
            $"模式={session.Mode}；会话={session.SessionId}");
        RefreshLogBindings();
        return true;
    }
    public bool CancelCameraNavigation(string reason)
    {
        if (_cameraSession is not { } session) return false;
        if (session.Mode == CameraSessionMode.Orbit) EndFrozenOrbit();
        ApplyCameraFrame(new(session.StartCamera, session.StartCenter), "ViewReset");
        _cameraSession = null;
        PublishSceneRenderSnapshot();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, "相机会话已取消",
            $"原因={reason}；会话={session.SessionId}");
        RefreshLogBindings();
        return true;
    }
    bool TryPreviewFrozenOrbit(CameraSessionSnapshot session, double dx, double dy,
        out CameraFrameResult result)
    {
        if (session.FrozenOrbit is null) { result = default; return false; }
        var ok = session.FrozenOrbit.TryMoveTo(dx * 0.008, dy * 0.006, _cameraRevision + 1,
            out result, out _);
        if (ok) AddFrozenOrbitProbe(OrbitProbePhase.Move, new(dx, dy, 0));
        return ok;
    }
    void EndFrozenOrbit()
    {
        AddFrozenOrbitProbe(OrbitProbePhase.End, Vector3d.Zero);
        _orbitSession = null;
    }
    void ApplyCameraResult(CameraFrameResult result)
    {
        ApplyCameraFrame(result, "OrbitOrPan");
        if (_activeViewFace != "默认视角" && _camera.Mode == ProjectionMode.Perspective)
        {
            _activeViewFace = "默认视角";
            OnPropertyChanged(nameof(ActiveViewFace));
        }
        OnPropertyChanged(nameof(NavigationCamera));
        PublishSceneRenderSnapshot();
    }
    void ApplyCameraFrame(CameraFrameResult result, string writer)
    {
        ViewportProbe.CameraWriter(writer, _camera, _observationCenter, result.Camera, result.ObservationCenter);
        _camera = result.Camera;
        _observationCenter = result.ObservationCenter;
        ViewportProbe.CameraSnapshot("T1_CAMERA_AFTER", _camera, _observationCenter);
    }
}
