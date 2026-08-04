using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

// F3-F4：取景命令。正交模式保持正交（尺度按包围范围适配），透视模式沿用距离构图。
public sealed partial class UiVm
{
    void FrameAllCamera(string source)
    {
        var frame = _camera.Mode == ProjectionMode.Orthographic
            ? EditorCameraFraming.FrameOrthographicWithCenter(
                _sceneState.RenderSnapshot.Entities.Select(e => e.Transform.Position),
                _camera.Forward, _camera.Up, _viewportAspect,
                _camera.Position.DistanceTo(_observationCenter), ++_cameraRevision)
            : EditorCameraFraming.FrameAllWithCenter(
                _sceneState.RenderSnapshot.Entities.Select(e => e.Transform.Position),
                _viewportAspect, ++_cameraRevision);
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

        var frame = _camera.Mode == ProjectionMode.Orthographic
            ? EditorCameraFraming.FrameOrthographicWithCenter(
                [entity.Transform.Position], _camera.Forward, _camera.Up, _viewportAspect,
                _camera.Position.DistanceTo(_observationCenter), ++_cameraRevision)
            : EditorCameraFraming.FrameSelectedWithCenter(
                entity.Transform.Position, _viewportAspect, ++_cameraRevision);
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        PublishSceneRenderSnapshot();
        FooterMessage = $"聚焦：{EditorDisplayText.Entity(EntityId.FromInt(key.Value))} 已进入视野。";
    }
}
