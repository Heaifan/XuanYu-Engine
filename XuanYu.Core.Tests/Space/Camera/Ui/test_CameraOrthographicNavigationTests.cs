using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

// F3-F4：正交导航语义（Dolly 缩放尺度不动位置、Pan 保持正交、Orbit 恢复透视）+ 正交视图工厂。
public sealed class CameraOrthographicNavigationTests
{
    [Fact]
    public void Orthographic_dolly_zooms_scale_without_moving_position()
    {
        var start = OrthoCamera(scale: 10.0);

        Assert.True(CameraNavigation.TryDolly(start, Vector3d.Zero, 1.0, 2, out var result, out _));
        Assert.Equal(ProjectionMode.Orthographic, result.Camera.Mode);
        Assert.Equal(8.5, result.Camera.OrthographicScale, 9);
        Assert.Equal(start.Position, result.Camera.Position);
    }

    [Fact]
    public void Orthographic_dolly_clamps_scale_bounds()
    {
        var start = OrthoCamera(scale: 0.0005);

        Assert.True(CameraNavigation.TryDolly(start, Vector3d.Zero, 1.0, 2, out var result, out _));
        Assert.Equal(0.001, result.Camera.OrthographicScale, 9);
    }

    [Fact]
    public void Orthographic_pan_keeps_mode_and_moves_center()
    {
        var start = OrthoCamera(scale: 10.0);

        Assert.True(CameraNavigation.TryPan(start, Vector3d.Zero, 10.0, 0.0, 100, 2, out var result, out _));
        Assert.Equal(ProjectionMode.Orthographic, result.Camera.Mode);
        Assert.Equal(10.0, result.Camera.OrthographicScale, 9);
        Assert.True(result.Camera.Position.DistanceTo(new Vector3d(1, 0, -5)) < 0.0001);
        Assert.True(result.ObservationCenter.DistanceTo(new Vector3d(1, 0, 0)) < 0.0001);
    }

    [Fact]
    public void Orthographic_orbit_restores_perspective()
    {
        var start = OrthoCamera(scale: 10.0);

        Assert.True(CameraNavigation.TryOrbit(start, Vector3d.Zero, 1.0, 0.0, 2, out var result, out _));
        Assert.Equal(ProjectionMode.Perspective, result.Camera.Mode);
    }

    [Fact]
    public void Orthographic_view_factory_computes_scale_from_distance_and_fov()
    {
        // 60° 垂直 FOV、距离 10 → 可见竖直范围 = 2×10×tan(30°)。
        Assert.Equal(11.547005, OrthographicViewFactory.ScaleForDistance(10.0, 60.0), 4);
        Assert.Throws<ArgumentOutOfRangeException>(() => OrthographicViewFactory.ScaleForDistance(0.0, 60.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => OrthographicViewFactory.ScaleForDistance(10.0, 180.0));
    }

    static CameraState OrthoCamera(double scale) => new(
        new Vector3d(0, 0, -5), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 1,
        ProjectionMode.Orthographic, scale);
}
