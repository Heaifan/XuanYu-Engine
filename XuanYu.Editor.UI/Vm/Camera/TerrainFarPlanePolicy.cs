using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Editor.UI;

internal static class TerrainFarPlanePolicy
{
    const double AbsoluteMarginMeters = 50.0;
    const double RelativeMargin = 0.05;

    public static double Resolve(CameraState camera, ViewportState viewport,
        IReadOnlyList<SpatialAabb> bounds, IReadOnlyList<SpatialAabb>? sceneBounds = null)
    {
        if (bounds.Count == 0 && (sceneBounds is null || sceneBounds.Count == 0) ||
            viewport.LogicalWidth <= 0 || viewport.LogicalHeight <= 0)
            return camera.FarPlane;
        var aspect = viewport.LogicalWidth / viewport.LogicalHeight;
        var tanY = camera.Mode == ProjectionMode.Perspective
            ? Math.Tan(camera.VerticalFovDegrees * Math.PI / 360.0) : 0.0;
        var tanX = tanY * aspect;
        var maxDepth = 0.0;
        Measure(bounds, camera, tanX, tanY, aspect, ref maxDepth);
        if (sceneBounds is not null) Measure(sceneBounds, camera, tanX, tanY, aspect, ref maxDepth);
        if (maxDepth <= camera.NearPlane || !double.IsFinite(maxDepth)) return camera.FarPlane;
        var margin = Math.Max(AbsoluteMarginMeters, maxDepth * RelativeMargin);
        var far = maxDepth + margin;
        return double.IsFinite(far) ? Math.Max(camera.NearPlane * 10.0, far) : camera.FarPlane;
    }

    static void Measure(IReadOnlyList<SpatialAabb> bounds, CameraState camera,
        double tanX, double tanY, double aspect, ref double maxDepth)
    {
        foreach (var box in bounds)
        {
            if (!IntersectsInfiniteFrustum(camera, box, tanX, tanY, aspect)) continue;
            var center = (box.Min + box.Max) * 0.5 - camera.Position;
            var extents = (box.Max - box.Min) * 0.5;
            maxDepth = Math.Max(maxDepth, center.Dot(camera.Forward) +
                Math.Abs(camera.Forward.X) * extents.X + Math.Abs(camera.Forward.Y) * extents.Y +
                Math.Abs(camera.Forward.Z) * extents.Z);
        }
    }

    static bool IntersectsInfiniteFrustum(CameraState c, SpatialAabb b,
        double tanX, double tanY, double aspect)
    {
        var center = (b.Min + b.Max) * 0.5 - c.Position;
        var e = (b.Max - b.Min) * 0.5;
        if (c.Mode == ProjectionMode.Orthographic)
        {
            var halfWidth = c.OrthographicScale * aspect * 0.5;
            var halfHeight = c.OrthographicScale * 0.5;
            return !Outside(c.Forward, center, e, -c.NearPlane) &&
                !Outside(c.Right, center, e, halfWidth) &&
                !Outside(-c.Right, center, e, halfWidth) &&
                !Outside(c.Up, center, e, halfHeight) &&
                !Outside(-c.Up, center, e, halfHeight);
        }
        return !Outside(c.Forward, center, e, -c.NearPlane) &&
            !Outside(c.Forward * tanX + c.Right, center, e, 0) &&
            !Outside(c.Forward * tanX - c.Right, center, e, 0) &&
            !Outside(c.Forward * tanY + c.Up, center, e, 0) &&
            !Outside(c.Forward * tanY - c.Up, center, e, 0);
    }

    static bool Outside(Vector3d normal, Vector3d center, Vector3d extents, double offset) =>
        center.Dot(normal) + offset + Math.Abs(normal.X) * extents.X +
        Math.Abs(normal.Y) * extents.Y + Math.Abs(normal.Z) * extents.Z < 0;
}
