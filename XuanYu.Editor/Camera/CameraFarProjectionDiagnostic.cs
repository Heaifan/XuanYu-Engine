using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public readonly record struct CameraFarProjectionDiagnostic(
    double Distance, double CenterRayT, double MetersPerDipX, double MetersPerDipY,
    bool MetricValid)
{
    public static CameraFarProjectionDiagnostic Create(CameraState camera,
        Vector3d target, ViewportState? viewport)
    {
        var distance = camera.Position.DistanceTo(target);
        if (!TryHit(camera.Position, camera.Forward, out var center, out var t) || viewport is null)
            return new(distance, t, double.NaN, double.NaN, false);
        var right = Ray(camera, viewport.Value, 2.0 / viewport.Value.LogicalWidth, 0.0);
        var down = Ray(camera, viewport.Value, 0.0, -2.0 / viewport.Value.LogicalHeight);
        if (!TryHit(right.Origin, right.Direction, out var x, out _) ||
            !TryHit(down.Origin, down.Direction, out var y, out _))
            return new(distance, t, double.NaN, double.NaN, false);
        var xMetric = center.DistanceTo(x); var yMetric = center.DistanceTo(y);
        var valid = double.IsFinite(xMetric) && xMetric > 0.0 &&
            double.IsFinite(yMetric) && yMetric > 0.0;
        return new(distance, t, xMetric, yMetric, valid);
    }

    static (Vector3d Origin, Vector3d Direction) Ray(CameraState camera,
        ViewportState viewport, double ndcX, double ndcY)
    {
        if (camera.Mode == ProjectionMode.Orthographic)
        {
            var halfHeight = camera.OrthographicScale * 0.5;
            return (camera.Position + (camera.Right * (ndcX * halfHeight *
                (viewport.LogicalWidth / viewport.LogicalHeight))) + (camera.Up * (ndcY * halfHeight)),
                camera.Forward);
        }
        var tangent = System.Math.Tan(camera.VerticalFovDegrees * System.Math.PI / 360.0);
        return (camera.Position, (camera.Forward + (camera.Right * (ndcX *
            (viewport.LogicalWidth / viewport.LogicalHeight) * tangent)) + (camera.Up * (ndcY * tangent))).Normalize());
    }

    static bool TryHit(Vector3d origin, Vector3d direction, out Vector3d hit, out double t)
    {
        t = double.NaN; hit = default;
        if (System.Math.Abs(direction.Z) < 0.001) return false;
        t = -origin.Z / direction.Z;
        if (!double.IsFinite(t) || t <= 0.0) return false;
        hit = origin + (direction * t);
        return true;
    }
}
