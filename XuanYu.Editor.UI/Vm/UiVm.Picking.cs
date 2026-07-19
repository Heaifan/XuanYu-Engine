using XuanYu.Core.Math;
using XuanYu.Core.Picking;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    long _pickSequence;

    public bool PickViewportPointer(double x, double y, int logicalW, int logicalH, int physicalW, int physicalH, double dpi, long viewportRevision, bool hostValid)
    {
        if (!hostValid || x < 0 || y < 0 || x > logicalW || y > logicalH) return false;
        var request = new ViewportPickingRequest(
            ++_pickSequence,
            new ViewportState(0, 0, logicalW, logicalH, physicalW, physicalH, dpi, viewportRevision),
            DefaultPickingCamera(viewportRevision),
            x,
            y,
            SpatialQueryCategory.SceneEntity,
            _sceneState.SpatialRevision);
        var result = ViewportPickingService.Pick(
            request,
            _sceneState.RaycastSpatial,
            () => viewportRevision,
            () => _sceneState.SpatialRevision);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            ViewportPickingLogFormatter.Message(result),
            ViewportPickingLogFormatter.Detail(result, x, y, dpi));
        ApplyViewportSelection(result);
        RefreshLogBindings();
        return true;
    }

    static CameraState DefaultPickingCamera(long revision) =>
        new(new Vector3d(0, 0, -5), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, revision);
}
