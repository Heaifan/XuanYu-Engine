using XuanYu.Core.Math;
using XuanYu.Editor.Camera;

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
        _cameraSession = new CameraSessionSnapshot(
            ++_cameraSessionRevision, pointerId, mode, x, y, _camera, _observationCenter, width, height);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "相机会话开始", $"模式={mode}；会话={_cameraSession.SessionId}");
        RefreshLogBindings();
        return true;
    }

    public bool PreviewCameraNavigation(long pointerId, double x, double y)
    {
        if (_cameraSession is not { } session || session.PointerId != pointerId) return false;
        var dx = x - session.StartX;
        var dy = y - session.StartY;
        // F3-F2：失败安全——Try* 成功才应用；失败保留上次预览且不递增 Revision。
        var ok = session.Mode == CameraSessionMode.Orbit
            ? CameraNavigation.TryOrbit(session.StartCamera, session.StartCenter, dx, dy,
                _cameraRevision + 1, out var result, out _)
            : CameraNavigation.TryPan(session.StartCamera, session.StartCenter, dx, dy, session.Height,
                _cameraRevision + 1, out result, out _);
        if (!ok) return false;
        _cameraRevision = result.Camera.Revision;
        ApplyCameraResult(result);
        return true;
    }

    public bool EndCameraNavigation(long pointerId)
    {
        if (_cameraSession is not { } session || session.PointerId != pointerId) return false;
        _cameraSession = null;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "相机会话结束", $"模式={session.Mode}；会话={session.SessionId}");
        RefreshLogBindings();
        return true;
    }

    public bool DollyCamera(double wheelDelta)
    {
        if (_cameraSession is not null || _editorState.InteractionSnapshot.HasCapture) return false;
        // F3-F2：失败安全——成功才替换相机/中心/Revision；失败保留旧状态并记录，不让异常逃出输入循环。
        if (!CameraNavigation.TryDolly(_camera, _observationCenter, wheelDelta,
            _cameraRevision + 1, out var result, out var reason))
        {
            _logBus.Error(EditorLogSource.Input, EditorLogCategory.Command, "相机 Dolly 失败", reason);
            return false;
        }

        _cameraRevision = result.Camera.Revision;
        ApplyCameraResult(result);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command, "相机 Dolly 已执行", $"滚轮={wheelDelta:g}");
        RefreshLogBindings();
        return true;
    }

    public bool CancelCameraNavigation(string reason)
    {
        if (_cameraSession is not { } session) return false;
        _camera = session.StartCamera;
        _observationCenter = session.StartCenter;
        _cameraSession = null;
        PublishSceneRenderSnapshot();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "相机会话已取消", $"原因={reason}；会话={session.SessionId}");
        RefreshLogBindings();
        return true;
    }

    void ApplyCameraResult(CameraFrameResult result)
    {
        _camera = result.Camera;
        _observationCenter = result.ObservationCenter;
        // F3-D3：导航 Gizmo 实时跟随相机姿态。
        OnPropertyChanged(nameof(NavigationCamera));
        PublishSceneRenderSnapshot();
    }
}
