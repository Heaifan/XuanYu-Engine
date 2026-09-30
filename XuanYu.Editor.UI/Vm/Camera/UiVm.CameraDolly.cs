using XuanYu.Editor.Camera;

using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool DollyCamera(double wheelDelta)
        => DollyCameraCore(wheelDelta, null, null, null);

    public bool DollyCameraAtCursor(double wheelDelta, double cursorX, double cursorY, ViewportState viewport)
        => DollyCameraCore(wheelDelta, cursorX, cursorY, viewport);

    bool DollyCameraCore(double wheelDelta, double? cursorX, double? cursorY, ViewportState? viewport)
    {
        if (_cameraSession is not null || _editorState.InteractionSnapshot.HasCapture) return false;
        var anchor = cursorX is { } x && cursorY is { } y && viewport is { } v
            ? TryResolveZoomAnchor(x, y, v)
            : null;
        if (!CameraNavigation.TryDolly(_camera, _observationCenter, wheelDelta,
            _cameraRevision + 1, anchor,
            out var result, out var reason))
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

    Vector3d? TryResolveZoomAnchor(double x, double y, ViewportState viewport)
    {
        if (x < viewport.LogicalX || y < viewport.LogicalY ||
            x > viewport.LogicalX + viewport.LogicalWidth ||
            y > viewport.LogicalY + viewport.LogicalHeight)
            return null;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var ray = WorldRayFactory.FromViewportPoint(projection, x, y);
        if (TerrainWorld is { } world)
        {
            var terrain = new TerrainWorldGroundSurface(world);
            var terrainHit = GroundPickResolver.Resolve(ray, terrain, MapSession.CurrentMap.Surface.BaseHeightMeters);
            return terrainHit.IsValid
                ? new Vector3d(terrainHit.WorldXY.X, terrainHit.WorldXY.Y, terrainHit.ResolvedElevation)
                : null;
        }
        var referenceHit = GroundPickResolver.Resolve(ray, null, MapSession.CurrentMap.Surface.BaseHeightMeters);
        return referenceHit.IsValid
            ? new Vector3d(referenceHit.WorldXY.X, referenceHit.WorldXY.Y, referenceHit.ResolvedElevation)
            : null;
    }
}
