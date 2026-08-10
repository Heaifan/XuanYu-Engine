using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapEditorZoomPolicyTests
{
    [Fact]
    public void Orthographic_metric_failure_holds_previous_legal_camera()
    {
        var viewport = Viewport();
        var start = Camera(600, ProjectionMode.Orthographic);
        var candidate = Camera(0.001, ProjectionMode.Orthographic);
        var result = MapEditorZoomPolicy.Clamp(start, new CameraFrameResult(candidate, Vector3d.Zero),
            Vector3d.Zero, viewport, 0.0, out var clamped);
        Assert.True(clamped);
        Assert.Equal(start.OrthographicScale, result.Camera.OrthographicScale, 6);
    }

    [Fact]
    public void Perspective_zoom_is_clamped_without_changing_generic_camera_limits()
    {
        var viewport = Viewport();
        var start = Camera(1000, ProjectionMode.Perspective);
        Assert.True(CameraNavigation.TryDolly(start, Vector3d.Zero, 10, 2,
            out var candidate, out _));
        var result = MapEditorZoomPolicy.Clamp(start, candidate, Vector3d.Zero,
            viewport, 0.0, out var clamped);
        Assert.True(clamped);
        Assert.True(ViewportMetricScale.TryCreate(ToRender(result.Camera), viewport, 0, out var metric));
        Assert.InRange(metric.MetersPerDip, MapEditorZoomPolicy.MinMetersPerDip, 0.626);
        Assert.Equal(0.25, CameraNavigation.Dolly(start, Vector3d.Zero, 100, 3).Camera.Position.DistanceTo(Vector3d.Zero), 6);
    }

    [Fact]
    public void Perspective_metric_failure_holds_previous_legal_camera()
    {
        var viewport = Viewport();
        var start = Camera(600, ProjectionMode.Perspective);
        var candidate = new CameraState(new Vector3d(0, 0, 100), Vector3d.UnitX,
            Vector3d.UnitZ, 60, 0.1, 10000, 1);
        var result = MapEditorZoomPolicy.Clamp(start, new CameraFrameResult(candidate, Vector3d.Zero),
            Vector3d.Zero, viewport, 0, out var clamped);
        Assert.True(clamped);
        Assert.Equal(start, result.Camera);
    }

    static ViewportState Viewport() => new(0, 0, 800, 600, 800, 600, 1, 1);

    static CameraState Camera(double distance, ProjectionMode mode) => new(
        new Vector3d(0, 0, distance), new Vector3d(0, 0, -1), new Vector3d(0, 1, 0),
        60, 0.1, 10000, 1, mode, mode == ProjectionMode.Orthographic ? distance : 0);

    static RenderCameraProjection ToRender(CameraState camera) => new(
        camera.Position, camera.Forward, camera.Up, camera.VerticalFovDegrees,
        camera.NearPlane, camera.FarPlane, camera.Revision, camera.Mode, camera.OrthographicScale);
}
