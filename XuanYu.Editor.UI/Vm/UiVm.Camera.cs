using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    CameraState _camera = DefaultEditorCamera.Create(1);
    Vector3d _observationCenter = DefaultEditorCamera.Target;
    double _viewportAspect = 16.0 / 9.0;
    long _cameraRevision = 1;
    bool _viewportCameraFramed;

    CameraState CurrentCamera(long revision) => new(
        _camera.Position,
        _camera.Forward,
        _camera.Up,
        _camera.VerticalFovDegrees,
        _camera.NearPlane,
        _camera.FarPlane,
        Math.Max(revision, _camera.Revision));

    public void UpdateViewportFrame(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        if (_cameraSession is not null && global::System.Math.Abs(_viewportAspect - ((double)width / height)) > 0.000001)
            CancelCameraNavigation("Resize");
        _viewportAspect = (double)width / height;
        if (_viewportCameraFramed) return;
        _viewportCameraFramed = true;
        FrameAllCamera("启动看全");
    }

    void FrameAllCamera(string source)
    {
        var frame = EditorCameraFraming.FrameAllWithCenter(
            _sceneState.RenderSnapshot.Entities.Select(e => e.Transform.Position),
            _viewportAspect,
            ++_cameraRevision);
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        PublishSceneRenderSnapshot();
        FooterMessage = $"{source}：当前可见实体已进入视野。";
    }

    void FrameSelectedCamera()
    {
        if (!TrySelectedEntityKey(out var key) || !_sceneState.TryGetEntity(key, out var entity))
        {
            FrameAllCamera("未选中实体，查看全部");
            return;
        }

        var frame = EditorCameraFraming.FrameSelectedWithCenter(entity.Transform.Position,
            _viewportAspect, ++_cameraRevision);
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        PublishSceneRenderSnapshot();
        FooterMessage = $"聚焦：{EditorDisplayText.Entity(EntityId.FromInt(key.Value))} 已进入视野。";
    }
}
