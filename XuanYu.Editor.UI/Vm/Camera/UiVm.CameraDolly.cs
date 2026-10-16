using XuanYu.Editor.Camera;
using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World;
namespace XuanYu.Editor.UI;
public sealed partial class UiVm
{
    public bool DollyCamera(double wheelDelta)
        => DollyCameraCore(wheelDelta, null, null, null, "DollyCamera", out _);
    public bool DollyCamera(double wheelDelta, out bool constrained)
        => DollyCameraCore(wheelDelta, null, null, null, "DollyCamera", out constrained);
    public bool DollyCameraAtCursor(double wheelDelta, double cursorX, double cursorY, ViewportState viewport)
        => DollyCameraCore(wheelDelta, cursorX, cursorY, viewport, "DollyCameraAtCursor", out _);
    bool DollyCameraCore(double wheelDelta, double? cursorX, double? cursorY, ViewportState? viewport, string operation, out bool constrained)
    {
        constrained = false;
        if (_cameraSession is not null || _editorState.InteractionSnapshot.HasCapture) return false;
        ViewportProbe.Operation(operation);
        var hit = cursorX is { } x && cursorY is { } y && viewport is { } v
            ? ResolveZoomAnchor(x, y, v)
            : GroundPickResult.Invalid;
        Vector3d? anchor = hit.IsValid
            ? new Vector3d(hit.WorldXY.X, hit.WorldXY.Y, hit.ResolvedElevation)
            : null;
        ViewportProbe.Log("input-camera", $"[ZOOM-ANCHOR] Pointer=({cursorX:0.###},{cursorY:0.###});" +
            $"Source={(hit.IsValid ? hit.SurfaceBinding.Kind : "Invalid")};" +
            $"Hit={(hit.IsValid ? $"{anchor}" : "INVALID")};RenderOrigin=WorldSpace");
        if (!CameraNavigation.TryDolly(_camera, _observationCenter, wheelDelta,
            _cameraRevision + 1, anchor,
            out var result, out var reason))
        {
            _logBus.Error(EditorLogSource.Input, EditorLogCategory.Command, "相机 Dolly 失败", reason);
            RefreshLogBindings();
            return false;
        }
        if (cursorX is null)
        {
            var unconstrainedPosition = result.Camera.Position;
            result = ConstrainDollyToTerrain(result, wheelDelta);
            constrained = result.Camera.Position != unconstrainedPosition;
        }
        _cameraRevision = result.Camera.Revision;
        ApplyCameraResult(result);
        TraceFarDolly(new(_camera, _observationCenter));
        return true;
    }
    CameraFrameResult ConstrainDollyToTerrain(CameraFrameResult result, double wheelDelta)
    {
        if (TerrainWorld is not { } world || result.Camera.Mode != ProjectionMode.Perspective)
            return result;
        var start = _camera.Position;
        var end = result.Camera.Position;
        var surface = new TerrainWorldGroundSurface(world);
        var startQuery = surface.QuerySurface(new(start.X, start.Y));
        var startInside = startQuery.IsValid &&
            start.Z <= startQuery.SurfaceZ * VerticalExaggeration + result.Camera.NearPlane;
        var recoveringFromInside = wheelDelta < 0 && startInside;
        var previousClearance = recoveringFromInside
            ? start.Z - startQuery.SurfaceZ * VerticalExaggeration - result.Camera.NearPlane
            : 0.0;
        var step = Math.Max(1, Math.Min(world.Metadata.ResolutionX, world.Metadata.ResolutionY));
        var count = Math.Clamp((int)Math.Ceiling(start.DistanceTo(end) / step), 1, 64);
        var lastSafe = start;
        for (var i = 1; i <= count; i++)
        {
            var position = start + ((end - start) * (i / (double)count));
            var query = surface.QuerySurface(new(position.X, position.Y));
            if (query.Status == WorldQueryStatus.OutOfBounds) { lastSafe = position; continue; } else if (!query.IsValid) break;
            var clearance = position.Z - query.SurfaceZ * VerticalExaggeration - result.Camera.NearPlane;
            if (recoveringFromInside && clearance + 1e-6 < previousClearance) break;
            if (recoveringFromInside) { lastSafe = position; previousClearance = clearance; recoveringFromInside = clearance <= 0; continue; }
            if (clearance <= 0)
                break;
            lastSafe = position;
        }
        if (lastSafe == end) return result;
        var camera = result.Camera;
        return result with { Camera = new(lastSafe, camera.Forward, camera.Up,
            camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane,
            camera.Revision, camera.Mode, camera.OrthographicScale) };
    }
    GroundPickResult ResolveZoomAnchor(double x, double y, ViewportState viewport)
    {
        if (x < viewport.LogicalX || y < viewport.LogicalY ||
            x > viewport.LogicalX + viewport.LogicalWidth ||
            y > viewport.LogicalY + viewport.LogicalHeight)
            return GroundPickResult.Invalid;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var ray = WorldRayFactory.FromViewportPoint(projection, x, y);
        if (TerrainWorld is { } world)
        {
            var terrain = new TerrainWorldGroundSurface(world);
            var terrainHit = GroundPickResolver.Resolve(ray, terrain, MapSession.CurrentMap.Surface.BaseHeightMeters);
            if (terrainHit.IsValid) return terrainHit;
        }
        return GroundPickResolver.Resolve(ray, null, MapSession.CurrentMap.Surface.BaseHeightMeters);
    }
}
