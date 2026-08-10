using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

// MAP-A-R3-D2-F1-V3：只限制地图编辑器视觉尺度，不改变通用 CameraNavigation。
public static class MapEditorZoomPolicy
{
    public const double MinMetersPerDip = 100.0 / 160.0;

    public static CameraFrameResult Clamp(
        CameraState start, CameraFrameResult candidate, Vector3d center,
        ViewportState viewport, double referenceHeight, out bool clamped)
    {
        clamped = false;
        if (candidate.Camera.Mode == ProjectionMode.Orthographic)
        {
            if (!TryMetric(candidate.Camera, viewport, referenceHeight, out var nextMetric))
                return Hold(start, center, out clamped);
            if (nextMetric.MetersPerDip >= MinMetersPerDip) return candidate;
            if (!TryMetric(start, viewport, referenceHeight, out var startMetric) ||
                startMetric.MetersPerDip < MinMetersPerDip)
                return Hold(start, center, out clamped);
            clamped = true;
            return OrthoClamp(candidate, center, viewport, nextMetric);
        }
        if (!TryMetric(candidate.Camera, viewport, referenceHeight, out var nextPerspectiveMetric))
            return Hold(start, center, out clamped);
        if (nextPerspectiveMetric.MetersPerDip >= MinMetersPerDip) return candidate;
        if (!TryMetric(start, viewport, referenceHeight, out var previousMetric) ||
            previousMetric.MetersPerDip < MinMetersPerDip)
            return Hold(start, center, out clamped);
        clamped = true;
        var low = candidate.Camera.Position.DistanceTo(center);
        var high = Math.Max(start.Position.DistanceTo(center), low + 1.0);
        var foundValidHigh = false;
        for (var i = 0; i < 40; i++)
        {
            if (TryMetric(CameraAtDistance(candidate.Camera, center, high), viewport,
                    referenceHeight, out var metric) && metric.MetersPerDip >= MinMetersPerDip)
            {
                foundValidHigh = true;
                break;
            }
            high *= 2.0;
        }
        if (!foundValidHigh) return Hold(start, center, out clamped);
        for (var i = 0; i < 40; i++)
        {
            var distance = (low + high) * 0.5;
            var camera = CameraAtDistance(candidate.Camera, center, distance);
            if (TryMetric(camera, viewport, referenceHeight, out var metric) &&
                metric.MetersPerDip >= MinMetersPerDip) high = distance;
            else low = distance;
        }
        var position = center - (candidate.Camera.Forward * high);
        var result = new CameraState(position, candidate.Camera.Forward, candidate.Camera.Up,
            candidate.Camera.VerticalFovDegrees, candidate.Camera.NearPlane, candidate.Camera.FarPlane,
            candidate.Camera.Revision);
        return new CameraFrameResult(result, center);
    }

    static CameraFrameResult OrthoClamp(CameraFrameResult candidate, Vector3d center,
        ViewportState viewport, ViewportMetricScale metric)
    {
        var scale = candidate.Camera.OrthographicScale * MinMetersPerDip / metric.MetersPerDip;
        scale = Math.Max(scale, MinMetersPerDip * viewport.LogicalHeight);
        var camera = new CameraState(candidate.Camera.Position, candidate.Camera.Forward,
            candidate.Camera.Up, candidate.Camera.VerticalFovDegrees, candidate.Camera.NearPlane,
            candidate.Camera.FarPlane, candidate.Camera.Revision, ProjectionMode.Orthographic, scale);
        return new CameraFrameResult(camera, center);
    }

    static CameraFrameResult Hold(CameraState start, Vector3d center, out bool clamped)
    {
        clamped = true;
        return new CameraFrameResult(start, center);
    }

    static CameraState CameraAtDistance(CameraState source, Vector3d center, double distance) => new(
        center - (source.Forward * distance), source.Forward, source.Up,
        source.VerticalFovDegrees, source.NearPlane, source.FarPlane, source.Revision,
        source.Mode, source.OrthographicScale);

    static bool TryMetric(CameraState camera, ViewportState viewport, double height,
        out ViewportMetricScale metric) => ViewportMetricScale.TryCreate(
            new RenderCameraProjection(camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane, camera.Revision,
                camera.Mode, camera.OrthographicScale), viewport, height, out metric);
}
