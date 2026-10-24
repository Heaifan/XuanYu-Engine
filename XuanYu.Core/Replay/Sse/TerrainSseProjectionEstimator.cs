using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space.Diagnostics;

public static class TerrainSseProjectionEstimator
{
    public static bool CameraInside(ViewProjectionState state, SpatialAabb bounds)
    {
        var p = state.Camera.Position;
        return p.X >= bounds.Min.X && p.X <= bounds.Max.X && p.Y >= bounds.Min.Y &&
            p.Y <= bounds.Max.Y && p.Z >= bounds.Min.Z && p.Z <= bounds.Max.Z;
    }

    public static bool NearPlaneIntersects(ViewProjectionState state, SpatialAabb bounds)
    {
        var min = double.PositiveInfinity; var max = double.NegativeInfinity;
        for (var i = 0; i < 8; i++)
        {
            var p = TerrainFrustumCuller.Corner(bounds, i);
            var depth = (p - state.Camera.Position).Dot(state.Camera.Forward);
            min = global::System.Math.Min(min, depth); max = global::System.Math.Max(max, depth);
        }
        return min <= state.Camera.NearPlane && max >= state.Camera.NearPlane;
    }

    public static bool TryEstimateMaxPixelError(ViewProjectionState state,
        SpatialAabb bounds, double verticalErrorMeters, out double pixels)
    {
        pixels = 0;
        if (!double.IsFinite(verticalErrorMeters) || verticalErrorMeters < 0) return false;
        var scaleX = state.Viewport.PhysicalWidth / state.Viewport.LogicalWidth;
        var scaleY = state.Viewport.PhysicalHeight / state.Viewport.LogicalHeight;
        var z = (bounds.Min.Z + bounds.Max.Z) * 0.5;
        for (var yi = 0; yi < 3; yi++) for (var xi = 0; xi < 3; xi++)
        {
            var x = bounds.Min.X + (bounds.Max.X - bounds.Min.X) * xi / 2;
            var y = bounds.Min.Y + (bounds.Max.Y - bounds.Min.Y) * yi / 2;
            if (!state.TryProjectWorldPoint(new(x, y, z - verticalErrorMeters), out var a) ||
                !state.TryProjectWorldPoint(new(x, y, z + verticalErrorMeters), out var b)) return false;
            var dx = (a.X - b.X) * scaleX; var dy = (a.Y - b.Y) * scaleY;
            pixels = global::System.Math.Max(pixels, double.Hypot(dx, dy));
        }
        if (double.IsFinite(pixels)) return true;
        pixels = 0;
        return false;
    }
}
