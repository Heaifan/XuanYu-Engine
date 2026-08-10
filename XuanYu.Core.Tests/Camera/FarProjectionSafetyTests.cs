using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Camera;

public sealed class FarProjectionSafetyTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void Metric_try_create_does_not_throw_at_extreme_distance()
    {
        var camera = ExtremeCamera();
        var error = Record.Exception(() =>
            ViewportMetricScale.TryCreate(ToRender(camera), Viewport, 0.0, out _));
        Assert.Null(error);
    }

    [Fact]
    public void Far_diagnostic_uses_double_geometry_before_view_projection()
    {
        var data = CameraFarProjectionDiagnostic.Create(ExtremeCamera(), Vector3d.Zero, Viewport);
        Assert.True(data.MetricValid);
        Assert.True(data.Distance >= 1_000_000_000.0);
        Assert.True(data.CenterRayT > 0.0);
        Assert.True(data.MetersPerDipX > 0.0);
        Assert.True(data.MetersPerDipY > 0.0);
    }

    static CameraState ExtremeCamera() => new(new Vector3d(1_000_000_000.2, 0, 1_000_000_000.2),
        new Vector3d(-1, 0, -1).Normalize(), Vector3d.UnitZ, 60, 0.1, 4_000_000_000.0, 1);

    static RenderCameraProjection ToRender(CameraState camera) => new(camera.Position, camera.Forward,
        camera.Up, camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane, camera.Revision,
        camera.Mode, camera.OrthographicScale);
}
