using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

// F3-D2：导航 Gizmo 相机快照（Right/Up/Forward 投影输入；不含平移）。
public sealed record NavigationCameraSnapshot(
    Vector3d Position, Vector3d Right, Vector3d Up, Vector3d Forward, long Revision);

public sealed partial class UiVm
{
    CameraState _camera => _cameraController.Camera;
    Vector3d _observationCenter => _cameraController.ObservationCenter;
    double _viewportAspect => _cameraController.Aspect;
    double _viewportDpiScale => _cameraController.DpiScale;
    long _cameraRevision => _cameraController.Revision;
    bool _viewportCameraFramed => _cameraController.IsFramed;
    CameraSessionSnapshot? _cameraSession => _cameraController.Session;

    // F3-F1：DPI 更新（导航 Gizmo Overlay 用 DIP→像素换算；默认 1.0）。
    public void UpdateViewportDpi(double dpiScale)
    {
        if (!_cameraController.SetDpiScale(dpiScale)) return;
        if (_lastViewport is { } viewport)
            _lastViewport = new ViewportState(viewport.LogicalX, viewport.LogicalY,
                viewport.LogicalWidth, viewport.LogicalHeight,
                Math.Max(1, (int)Math.Round(viewport.LogicalWidth * dpiScale)),
                Math.Max(1, (int)Math.Round(viewport.LogicalHeight * dpiScale)),
                dpiScale, viewport.Revision);
        PublishSceneRenderSnapshot();
    }
    public NavigationCameraSnapshot NavigationCamera => new(
        _camera.Position, _camera.Right, _camera.Up, _camera.Forward, _cameraRevision);

    CameraState CurrentCamera(long revision) => new(
        _camera.Position,
        _camera.Forward,
        _camera.Up,
        _camera.VerticalFovDegrees,
        _camera.NearPlane,
        _camera.FarPlane,
        Math.Max(revision, _camera.Revision),
        _camera.Mode,
        _camera.OrthographicScale);

    public void UpdateViewportFrame(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        if (_editorState.InteractionSnapshot.HasCapture) CancelInteraction("Resize");
        var aspectChanged =
            global::System.Math.Abs(_viewportAspect - ((double)width / height)) > 0.000001;
        if (_cameraSession is not null && aspectChanged)
            CancelCameraNavigation("Resize");
        _cameraController.SetAspect((double)width / height);
        var physicalW = Math.Max(1, (int)Math.Round(width * _viewportDpiScale));
        var physicalH = Math.Max(1, (int)Math.Round(height * _viewportDpiScale));
        _lastViewport = new ViewportState(0, 0, width, height, physicalW, physicalH,
            _viewportDpiScale, Math.Max(_cameraRevision, 1));
        if (_viewportCameraFramed)
        {
            ApplyCameraFrame(new(_camera, _observationCenter), "ViewportResize");
            PublishSceneRenderSnapshot();
            return;
        }
        _cameraController.MarkFramed();
        FrameAllCamera("启动看全");
    }

    void ResetCameraForSceneReplacement(bool frameEntities = false)
    {
        if (frameEntities)
        {
            var frame = EditorCameraFraming.FrameAllWithCenter(
                _sceneState.RenderSnapshot.Entities.Select(e => e.Transform.Position), _viewportAspect, _cameraController.NextRevision());
            ApplyCameraFrame(frame, "FrameAll");
        }
        else
        {
            ApplyCameraFrame(new(DefaultEditorCamera.Create(_cameraController.NextRevision()), DefaultEditorCamera.Target), "ViewReset");
        }
        _cameraController.MarkFramed();
        PublishSceneRenderSnapshot();
    }
}
