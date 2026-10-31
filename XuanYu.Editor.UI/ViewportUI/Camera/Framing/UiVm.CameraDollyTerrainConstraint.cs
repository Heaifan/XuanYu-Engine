using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.MapEditing;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    CameraFrameResult ConstrainDollyToTerrain(CameraFrameResult result, double wheelDelta)
    {
        if (TerrainWorld is not { } world || result.Camera.Mode != ProjectionMode.Perspective) return result;
        var start = _camera.Position; var end = result.Camera.Position;
        var resources = CachedTerrainRenderResources();
        IGroundSurface surface = resources is { Count: > 0 }
            ? new TerrainRenderGroundSurface(resources, world.EditDelta.Revision)
            : new TerrainWorldGroundSurface(world);
        var startQuery = surface.QuerySurface(new(start.X, start.Y));
        var startInside = startQuery.IsValid &&
            start.Z <= startQuery.SurfaceZ * VerticalExaggeration + result.Camera.NearPlane;
        var recovering = wheelDelta < 0 && startInside;
        var previous = recovering ? start.Z - startQuery.SurfaceZ * VerticalExaggeration - result.Camera.NearPlane : 0;
        var step = Math.Max(1, Math.Min(world.Metadata.ResolutionX, world.Metadata.ResolutionY));
        var count = Math.Clamp((int)Math.Ceiling(start.DistanceTo(end) / step), 1, 64);
        var lastSafe = start;
        for (var i = 1; i <= count; i++)
        {
            var position = start + ((end - start) * (i / (double)count));
            var query = surface.QuerySurface(new(position.X, position.Y));
            if (query.Status == WorldQueryStatus.OutOfBounds) { lastSafe = position; continue; }
            if (!query.IsValid) break;
            var clearance = position.Z - query.SurfaceZ * VerticalExaggeration - result.Camera.NearPlane;
            if (recovering && clearance + 1e-6 < previous) break;
            if (recovering) { lastSafe = position; previous = clearance; recovering = clearance <= 0; continue; }
            if (clearance <= 0) break;
            lastSafe = position;
        }
        if (lastSafe == end) return result;
        var camera = result.Camera;
        return result with { Camera = new(lastSafe, camera.Forward, camera.Up,
            camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane,
            camera.Revision, camera.Mode, camera.OrthographicScale) };
    }
}
