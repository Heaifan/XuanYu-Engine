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
    CameraState _camera = DefaultEditorCamera.Create(1);
    Vector3d _observationCenter = DefaultEditorCamera.Target;
    double _viewportAspect = 16.0 / 9.0;
    double _viewportDpiScale = 1.0;
    long _cameraRevision = 1;
    bool _viewportCameraFramed;

    // F3-F1：DPI 更新（导航 Gizmo Overlay 用 DIP→像素换算；默认 1.0）。
    public void UpdateViewportDpi(double dpiScale)
    {
        if (!double.IsFinite(dpiScale) || dpiScale <= 0.0) return;
        if (System.Math.Abs(_viewportDpiScale - dpiScale) < 0.001) return;
        _viewportDpiScale = dpiScale;
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
        _viewportAspect = (double)width / height;
        _lastViewport = new ViewportState(0, 0, width, height, width, height, 1.0, Math.Max(_cameraRevision, 1));
        if (_viewportCameraFramed) return;
        _viewportCameraFramed = true;
        FrameAllCamera("启动看全");
    }

    void ResetCameraForSceneReplacement(bool frameEntities = false)
    {
        if (frameEntities)
        {
            var frame = EditorCameraFraming.FrameAllWithCenter(
                _sceneState.RenderSnapshot.Entities.Select(e => e.Transform.Position), _viewportAspect, ++_cameraRevision);
            _camera = frame.Camera;
            _observationCenter = frame.ObservationCenter;
        }
        else
        {
            _camera = DefaultEditorCamera.Create(++_cameraRevision);
            _observationCenter = DefaultEditorCamera.Target;
        }
        _viewportCameraFramed = true;
        PublishSceneRenderSnapshot();
    }
}
