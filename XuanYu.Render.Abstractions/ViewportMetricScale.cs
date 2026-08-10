using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

// MAP-A-R3-D2-F1：唯一视口公制尺度源；不依赖 Vulkan、Avalonia 或 World。
public readonly record struct ViewportMetricScale(
    double MetersPerDipX, double MetersPerDipY, double DpiScale)
{
    public double MetersPerDip => System.Math.Min(MetersPerDipX, MetersPerDipY);
    public double MetersPerPhysicalPixel => MetersPerDip / DpiScale;
    public double MetersPerPhysicalPixelX => MetersPerDipX / DpiScale;
    public double MetersPerPhysicalPixelY => MetersPerDipY / DpiScale;

    public static bool TryCreate(
        RenderCameraProjection camera, ViewportState viewport,
        double referenceHeight, out ViewportMetricScale metric)
    {
        metric = default;
        if (!double.IsFinite(referenceHeight) || viewport.LogicalHeight <= 0.0) return false;
        if (!ViewProjectionState.TryCreate(new CameraState(camera.Position, camera.Forward,
                camera.Up, camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane,
                camera.Revision, camera.Mode, camera.OrthographicScale), viewport, out var state))
            return false;
        var x = viewport.LogicalX + (viewport.LogicalWidth * 0.5);
        var y = viewport.LogicalY + (viewport.LogicalHeight * 0.5);
        if (!TryHitPlane(state!, x, y, referenceHeight, out var center) ||
            !TryHitPlane(state!, x + 1.0, y, referenceHeight, out var right) ||
            !TryHitPlane(state!, x, y + 1.0, referenceHeight, out var down)) return false;
        var metersPerDipX = center.DistanceTo(right);
        var metersPerDipY = center.DistanceTo(down);
        if (!double.IsFinite(metersPerDipX) || metersPerDipX <= 0.0 ||
            !double.IsFinite(metersPerDipY) || metersPerDipY <= 0.0) return false;
        var dpi = viewport.DpiScale;
        metric = new ViewportMetricScale(metersPerDipX, metersPerDipY, dpi);
        return true;
    }

    static bool TryHitPlane(
        ViewProjectionState state, double x, double y, double height, out Vector3d hit)
    {
        hit = default;
        var ray = WorldRayFactory.FromViewportPoint(state, x, y);
        if (System.Math.Abs(ray.Direction.Z) < 0.001) return false;
        var t = (height - ray.Origin.Z) / ray.Direction.Z;
        if (t <= 0.0) return false;
        hit = ray.Origin + (ray.Direction * t);
        return true;
    }
}
