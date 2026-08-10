using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryFrameDraftCamera()
    {
        var draft = _regionDrawing.Draft;
        if (draft is null || draft.Vertices.IsDefaultOrEmpty) return false;
        var height = MapSession.CurrentMap.Surface.BaseHeightMeters;
        var points = draft.Vertices
            .Select(point => new Vector3d(point.X, point.Y, height))
            .ToArray();
        var frame = _camera.Mode == ProjectionMode.Orthographic
            ? EditorCameraFraming.FrameOrthographicWithCenter(
                points, _camera.Forward, _camera.Up, _viewportAspect,
                _camera.Position.DistanceTo(_observationCenter), ++_cameraRevision, 75.0)
            : EditorCameraFraming.FrameDraftWithCenter(points, _viewportAspect, ++_cameraRevision);
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        PublishSceneRenderSnapshot();
        FooterMessage = "聚焦：当前区域草稿已进入视野。";
        FooterState = "状态：就绪";
        return true;
    }
}
