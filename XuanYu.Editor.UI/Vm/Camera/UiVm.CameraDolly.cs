using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool DollyCamera(double wheelDelta)
    {
        if (_cameraSession is not null || _editorState.InteractionSnapshot.HasCapture) return false;
        if (!CameraNavigation.TryDolly(_camera, _observationCenter, wheelDelta,
            _cameraRevision + 1, out var result, out var reason))
        {
            _logBus.Error(EditorLogSource.Input, EditorLogCategory.Command, "相机 Dolly 失败", reason);
            return false;
        }
        if (_lastViewport is { } viewport)
            result = MapEditorZoomPolicy.Clamp(_camera, result, _observationCenter, viewport,
                MapSession.CurrentMap.Surface.BaseHeightMeters, out _);
        _cameraRevision = result.Camera.Revision;
        ApplyCameraResult(result);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "相机 Dolly 已执行", $"滚轮={wheelDelta:g}");
        RefreshLogBindings();
        return true;
    }
}
