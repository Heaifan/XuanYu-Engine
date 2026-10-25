using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

// 比例尺语义：左下比例尺横条所在屏幕水平线，横条左右端点投影到参考地面。
public readonly record struct ScaleBarGroundMetric(
    bool IsValid, double MetersPerDip, double ReferenceY, double EndpointDistanceMeters);

public static class ScaleBarGroundProjection
{
    public static bool TryCreate(RenderCameraProjection camera, ViewportState viewport,
        double groundHeight, out ScaleBarGroundMetric metric)
        => TryCreate(camera, viewport, (_, _) => groundHeight, out metric);

    public static bool TryCreate(RenderCameraProjection camera, ViewportState viewport,
        Func<double, double, double?> surfaceHeight, out ScaleBarGroundMetric metric)
    {
        metric = default;
        ArgumentNullException.ThrowIfNull(surfaceHeight);
        if (viewport.LogicalWidth <= 0 || viewport.LogicalHeight <= 0)
            return false;
        if (!ViewProjectionState.TryCreate(new CameraState(camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane, camera.Revision,
                camera.Mode, camera.OrthographicScale), viewport, out var state) || state is null) return false;
        if (System.Math.Abs(camera.Forward.Z) < 0.05) return false;
        var y = viewport.LogicalY + viewport.LogicalHeight - 24.0;
        var center = viewport.LogicalX + 16.0 + 6.0 + ScaleIndicatorMetric.FixedBarWidthDip * 0.5;
        var half = ScaleIndicatorMetric.FixedBarWidthDip * 0.5;
        if (!TryHit(state, center - half, y, surfaceHeight, out var left) ||
            !TryHit(state, center + half, y, surfaceHeight, out var right)) return false;
        var distance = left.DistanceTo(right);
        if (!double.IsFinite(distance) || distance <= 0.0) return false;
        metric = new(true, distance / ScaleIndicatorMetric.FixedBarWidthDip, y, distance);
        return true;
    }

    static bool TryHit(ViewProjectionState state, double x, double y,
        Func<double, double, double?> surfaceHeight, out Vector3d hit)
    {
        hit = default;
        var ray = WorldRayFactory.FromViewportPoint(state, x, y);
        if (!double.IsFinite(ray.Direction.Z) || System.Math.Abs(ray.Direction.Z) < 0.001) return false;
        var initialHeight = surfaceHeight(ray.Origin.X, ray.Origin.Y) ?? 0.0;
        if (!double.IsFinite(initialHeight)) return false;
        var t = (initialHeight - ray.Origin.Z) / ray.Direction.Z;
        if (!double.IsFinite(t) || t <= 0.0) return false;
        for (var attempt = 0; attempt < 32; attempt++)
        {
            var point = ray.Origin + ray.Direction * t;
            var height = surfaceHeight(point.X, point.Y);
            if (height is not { } z || !double.IsFinite(z)) return false;
            var next = (z - ray.Origin.Z) / ray.Direction.Z;
            if (!double.IsFinite(next) || next <= 0.0) return false;
            if (System.Math.Abs(next - t) <= 1e-5)
            {
                hit = ray.Origin + ray.Direction * next;
                return double.IsFinite(hit.X) && double.IsFinite(hit.Y) && double.IsFinite(hit.Z);
            }
            t = next;
        }
        return false;
    }
}
