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
            RefreshLogBindings();
            return false;
        }
        TraceFarDolly(result);
        _cameraRevision = result.Camera.Revision;
        ApplyCameraResult(result);
        return true;
    }
}
