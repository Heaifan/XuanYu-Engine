using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.MapEditing;
using XuanYu.World;

namespace XuanYu.Editor.UI;

internal static class CameraTerrainConstraint
{
    public static CameraFrameResult Constrain(CameraState startCamera, CameraFrameResult result,
        double wheelDelta, double verticalExaggeration, IGroundSurface surface,
        double resolutionX, double resolutionY)
    {
        if (result.Camera.Mode != ProjectionMode.Perspective) return result;
        var start = startCamera.Position;
        var end = result.Camera.Position;
        var startQuery = surface.QuerySurface(new(start.X, start.Y));
        var startInside = startQuery.IsValid &&
            start.Z <= startQuery.SurfaceZ * verticalExaggeration + result.Camera.NearPlane;
        var recovering = wheelDelta < 0 && startInside;
        var previous = recovering
            ? start.Z - startQuery.SurfaceZ * verticalExaggeration - result.Camera.NearPlane
            : 0;
        var step = Math.Max(1, Math.Min(resolutionX, resolutionY));
        var count = Math.Clamp((int)Math.Ceiling(start.DistanceTo(end) / step), 1, 64);
        var lastSafe = start;
        for (var i = 1; i <= count; i++)
        {
            var position = start + ((end - start) * (i / (double)count));
            var query = surface.QuerySurface(new(position.X, position.Y));
            if (query.Status == WorldQueryStatus.OutOfBounds) { lastSafe = position; continue; }
            if (!query.IsValid) break;
            var clearance = position.Z - query.SurfaceZ * verticalExaggeration - result.Camera.NearPlane;
            if (recovering && clearance + 1e-6 < previous) break;
            if (recovering) { lastSafe = position; previous = clearance; recovering = clearance <= 0; continue; }
            if (clearance <= 0) break;
            lastSafe = position;
        }
        return lastSafe == end ? result : WithPosition(result, lastSafe);
    }

    static CameraFrameResult WithPosition(CameraFrameResult result, Vector3d position)
    {
        var camera = result.Camera;
        return result with { Camera = new(position, camera.Forward, camera.Up,
            camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane,
            camera.Revision, camera.Mode, camera.OrthographicScale) };
    }
}
