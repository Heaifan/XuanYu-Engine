using XuanYu.Core.Math;
using XuanYu.Core.Diagnostics;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string ScaleIndicatorText { get; private set; } = "";
    public double ScaleIndicatorWidthDip { get; private set; } = 80.0;
    public bool IsScaleIndicatorVisible { get; private set; }
    double _lastScaleDistanceMeters;

    void UpdateScaleIndicator()
    {
        if (_lastViewport is not { } viewport)
        {
            _lastScaleDistanceMeters = 0.0;
            SetScaleIndicator(false, "", 80.0);
            return;
        }
        var camera = new RenderCameraProjection(_camera.Position, _camera.Forward, _camera.Up,
            _camera.VerticalFovDegrees, _camera.NearPlane, _camera.FarPlane, _camera.Revision,
            _camera.Mode, _camera.OrthographicScale);
        var resources = IsTerrainContext ? CachedTerrainRenderResources() : null;
        bool hasGroundMetric;
        ScaleBarGroundMetric ground;
        if (resources is { Count: > 0 })
        {
            var surface = new TerrainRenderGroundSurface(resources,
                TerrainWorld?.EditDelta.Revision);
            hasGroundMetric = ScaleBarGroundProjection.TryCreate(camera, viewport,
                (x, y) =>
                {
                    var result = surface.QuerySurface(new MapPoint(x, y));
                    return result.IsValid ? result.SurfaceZ * VerticalExaggeration : null;
                }, out ground);
        }
        else
        {
            hasGroundMetric = ScaleBarGroundProjection.TryCreate(camera, viewport,
                MapSession.CurrentMap.Surface.BaseHeightMeters, out ground);
        }
        if (!hasGroundMetric)
        {
            _lastScaleDistanceMeters = 0.0;
            if (ViewportProbe.Enabled)
                ViewportProbe.Log("scale-indicator", $"[SCALE-GROUND-INVALID] CameraRevision={_camera.Revision};" +
                    $"Source={(resources is { Count: > 0 } ? "TerrainRenderHeightfieldTriangles" : "ReferencePlane")};" +
                    $"Viewport={viewport.PhysicalWidth}x{viewport.PhysicalHeight}");
            SetScaleIndicator(false, "", 80.0);
            return;
        }
        var bar = ScaleIndicatorMetric.FromMetersPerDip(
            ground.MetersPerDip, _lastScaleDistanceMeters);
        if (ViewportProbe.Enabled)
            ViewportProbe.Log("scale-indicator", $"[SCALE-GROUND] CameraRevision={_camera.Revision};" +
                $"Source={(resources is { Count: > 0 } ? "TerrainRenderHeightfieldTriangles" : "ReferencePlane")};" +
                $"EndpointDistanceMeters={ground.EndpointDistanceMeters:R};MetersPerDip={ground.MetersPerDip:R};" +
                $"DisplayDistanceMeters={bar.DistanceMeters:R};DisplayWidthDip={bar.WidthDip:R};Label={bar.Label};" +
                $"VerticalExaggeration={VerticalExaggeration:R};Viewport={viewport.PhysicalWidth}x{viewport.PhysicalHeight}");
        _lastScaleDistanceMeters = bar.DistanceMeters;
        SetScaleIndicator(bar.IsVisible, bar.Label, bar.WidthDip);
    }

    void SetScaleIndicator(bool visible, string text, double width)
    {
        if (IsScaleIndicatorVisible == visible && ScaleIndicatorText == text &&
            System.Math.Abs(ScaleIndicatorWidthDip - width) < 0.01) return;
        IsScaleIndicatorVisible = visible;
        ScaleIndicatorText = text;
        ScaleIndicatorWidthDip = width;
        OnPropertyChanged(nameof(IsScaleIndicatorVisible));
        OnPropertyChanged(nameof(ScaleIndicatorText));
        OnPropertyChanged(nameof(ScaleIndicatorWidthDip));
    }
}
