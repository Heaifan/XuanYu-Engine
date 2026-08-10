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
        if (!TryMetric(start, viewport, referenceHeight, out var startMetric) ||
            startMetric.MetersPerDip < MinMetersPerDip) return new CameraFrameResult(start, center);
        if (candidate.Camera.Mode == ProjectionMode.Orthographic)
        {
            if (candidate.Camera.OrthographicScale >= MinMetersPerDip * viewport.LogicalHeight)
                return candidate;
            clamped = true;
            return OrthoClamp(candidate, center, viewport);
        }
        if (!TryMetric(candidate.Camera, viewport, referenceHeight, out var nextMetric) ||
            nextMetric.MetersPerDip >= MinMetersPerDip) return candidate;
        clamped = true;
        var startDistance = start.Position.DistanceTo(center);
        var low = candidate.Camera.Position.DistanceTo(center);
        var high = startDistance;
        for (var i = 0; i < 40; i++)
        {
            var distance = (low + high) * 0.5;
            var camera = new CameraState(center - (candidate.Camera.Forward * distance),
                candidate.Camera.Forward, candidate.Camera.Up, candidate.Camera.VerticalFovDegrees,
                candidate.Camera.NearPlane, candidate.Camera.FarPlane, candidate.Camera.Revision);
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

    static CameraFrameResult OrthoClamp(CameraFrameResult candidate, Vector3d center, ViewportState viewport)
    {
        var scale = Math.Max(candidate.Camera.OrthographicScale, MinMetersPerDip * viewport.LogicalHeight);
        var camera = new CameraState(candidate.Camera.Position, candidate.Camera.Forward,
            candidate.Camera.Up, candidate.Camera.VerticalFovDegrees, candidate.Camera.NearPlane,
            candidate.Camera.FarPlane, candidate.Camera.Revision, ProjectionMode.Orthographic, scale);
        return new CameraFrameResult(camera, center);
    }

    static bool TryMetric(CameraState camera, ViewportState viewport, double height,
        out ViewportMetricScale metric) => ViewportMetricScale.TryCreate(
            new RenderCameraProjection(camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane, camera.Revision,
                camera.Mode, camera.OrthographicScale), viewport, height, out metric);
}
