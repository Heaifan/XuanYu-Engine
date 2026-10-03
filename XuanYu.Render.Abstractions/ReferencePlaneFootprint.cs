using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

public readonly record struct ReferencePlaneFootprint(double MinX, double MaxX, double MinY, double MaxY)
{
    public double Width => MaxX - MinX;
    public double Depth => MaxY - MinY;
    public double CenterX => (MinX + MaxX) * 0.5;
    public double CenterY => (MinY + MaxY) * 0.5;
    public bool Contains(Vector3d point) => point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;

    public static bool TryCreate(ViewProjectionState state, double height, out ReferencePlaneFootprint footprint)
    {
        Span<Vector3d> points = stackalloc Vector3d[9];
        Span<double> distances = stackalloc double[9];
        var v = state.Viewport; var valid = 0; var maxDistance = 0.0;
        for (var i = 0; i < 9; i++)
        {
            var x = v.LogicalX + (i % 3) * v.LogicalWidth / 2.0;
            var y = v.LogicalY + (i / 3) * v.LogicalHeight / 2.0;
            var ray = WorldRayFactory.FromViewportPoint(state, x, y);
            if (!TryHit(ray, height, state.Camera.FarPlane, out var hit, out var distance)) continue;
            points[i] = hit; distances[i] = distance; valid++;
            if (distance > maxDistance) maxDistance = distance;
        }
        if (valid == 0) { footprint = default; return false; }
        var visualDistance = ReferencePlaneHorizonCoverage.VisualGroundDistance(state, height, maxDistance);
        for (var i = 0; i < 9; i++)
        {
            if (distances[i] > 0) continue;
            var x = v.LogicalX + (i % 3) * v.LogicalWidth / 2.0;
            var y = v.LogicalY + (i / 3) * v.LogicalHeight / 2.0;
            points[i] = ReferencePlaneHorizonCoverage.Surrogate(WorldRayFactory.FromViewportPoint(state, x, y), height, visualDistance);
        }
        var minX = points[0].X; var maxX = minX; var minY = points[0].Y; var maxY = minY;
        for (var i = 1; i < 9; i++) { minX = System.Math.Min(minX, points[i].X); maxX = System.Math.Max(maxX, points[i].X); minY = System.Math.Min(minY, points[i].Y); maxY = System.Math.Max(maxY, points[i].Y); }
        footprint = new(minX, maxX, minY, maxY);
        return double.IsFinite(minX) && double.IsFinite(maxX) && double.IsFinite(minY) && double.IsFinite(maxY);
    }

    static bool TryHit(WorldRay ray, double height, double farPlane, out Vector3d hit, out double distance)
    {
        hit = default; distance = 0;
        if (!double.IsFinite(ray.Direction.Z) || System.Math.Abs(ray.Direction.Z) < ReferencePlaneHorizonCoverage.ParallelEpsilon) return false;
        distance = (height - ray.Origin.Z) / ray.Direction.Z;
        if (!double.IsFinite(distance) || distance <= 0 || distance > farPlane) { distance = 0; return false; }
        hit = ray.Origin + ray.Direction * distance; return true;
    }
}
