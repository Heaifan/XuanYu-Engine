using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    CameraState _camera = DefaultEditorCamera.Create(1);
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
        _viewportAspect = (double)width / height;
        if (_viewportCameraFramed) return;
        _viewportCameraFramed = true;
        FrameAllCamera("启动看全");
    }

    void FrameAllCamera(string source)
    {
        _camera = EditorCameraFraming.FrameAll(
            _sceneState.RenderSnapshot.Entities.Select(e => e.Transform.Position),
            _viewportAspect,
            ++_cameraRevision);
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

        _camera = EditorCameraFraming.FrameSelected(entity.Transform.Position,
            _viewportAspect, ++_cameraRevision);
        PublishSceneRenderSnapshot();
        FooterMessage = $"聚焦：{EditorDisplayText.Entity(EntityId.FromInt(key.Value))} 已进入视野。";
    }
}
