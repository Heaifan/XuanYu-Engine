using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public Vector3d ObservationCenter => _observationCenter;
    public bool IsCameraNavigationActive => _cameraSession is not null;
    public bool BeginCameraNavigation(long pointerId, double x, double y, bool shift, int width, int height)
    {
        if (_editorState.InteractionSnapshot.HasCapture) return false;
        var mode = shift ? CameraSessionMode.Pan : CameraSessionMode.Orbit;
        var orbit = mode == CameraSessionMode.Orbit ? ResolveOrbitPivot() : null;
        if (!_cameraController.TryBeginNavigation(pointerId, x, y, mode, width, height, orbit)) return false;
        var session = _cameraSession!;
        if (orbit is not null) StartOrbitProbe(session.SessionId, orbit);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, "相机会话开始",
            $"模式={mode}；会话={session.SessionId}");

        return true;
    }
    public bool PreviewCameraNavigation(long pointerId, double x, double y)
    {
        if (!_cameraController.TryPreviewNavigation(pointerId, x, y,
            out var result, out var dx, out var dy)) return false;
        if (_cameraSession?.Mode == CameraSessionMode.Orbit)
            AddFrozenOrbitProbe(OrbitProbePhase.Move, new(dx, dy, 0));
        ApplyCameraResult(result);
        return true;
    }
    public bool EndCameraNavigation(long pointerId)
    {
        if (!_cameraController.TryEndNavigation(pointerId, out var session)) return false;
        if (session.Mode == CameraSessionMode.Orbit) EndFrozenOrbit();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, "相机会话结束",
            $"模式={session.Mode}；会话={session.SessionId}");

        return true;
    }
    public bool CancelCameraNavigation(string reason)
    {
        if (!_cameraController.TryCancelNavigation(out var session, out var restore)) return false;
        if (session.Mode == CameraSessionMode.Orbit) EndFrozenOrbit();
        ApplyCameraFrame(restore, "ViewReset");
        PublishSceneRenderSnapshot();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, "相机会话已取消",
            $"原因={reason}；会话={session.SessionId}");

        return true;
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
        var finalCamera = ApplyFarPlanePolicy(result.Camera);
        ViewportProbe.CameraWriter(writer, _camera, _observationCenter, finalCamera, result.ObservationCenter);
        _cameraController.ApplyFrame(new(finalCamera, result.ObservationCenter));
        ViewportProbe.CameraSnapshot("T1_CAMERA_AFTER", _camera, _observationCenter);
    }
}
