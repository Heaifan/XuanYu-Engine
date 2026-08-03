using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

// F3-F4：正交投影契约（模式校验/射线/尺度投影/往返/深度/Fov 无关）。
public sealed class CameraOrthographicTests
{
    [Fact]
    public void Orthographic_camera_requires_positive_scale_and_defaults_perspective()
    {
        var perspective = new CameraState(TestPosition, Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0);
        Assert.Equal(ProjectionMode.Perspective, perspective.Mode);
        Assert.Equal(0.0, perspective.OrthographicScale);

        var ortho = OrthographicCamera();
        Assert.Equal(ProjectionMode.Orthographic, ortho.Mode);
        Assert.Equal(2.0, ortho.OrthographicScale);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CameraState(TestPosition, Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0, ProjectionMode.Orthographic, 0.0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CameraState(TestPosition, Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0, ProjectionMode.Orthographic, double.NaN));
    }

    [Fact]
    public void Orthographic_center_ray_matches_forward_and_origin_at_near()
    {
        var state = ViewProjectionState.Create(OrthographicCamera(), TestViewport(800, 600));
        var ray = WorldRayFactory.FromViewportPoint(state, 400, 300);

        SpaceAssert.Near(Vector3d.UnitZ, ray.Direction);
        Assert.True(ray.Origin.DistanceTo(new Vector3d(0, 0, -4.9)) < 0.0001);
    }

    [Fact]
    public void Orthographic_corner_rays_are_parallel_and_freeze_screen_axis_convention()
    {
        var state = ViewProjectionState.Create(OrthographicCamera(), TestViewport(800, 600));
        var rightTop = WorldRayFactory.FromViewportPoint(state, 800, 0);

        // 正交下所有射线方向一致（平行投影），角落差异只体现在起点。
        // 屏幕约定与透视一致：屏幕右上 = 世界 (-X, +Y)。
        SpaceAssert.Near(Vector3d.UnitZ, rightTop.Direction);
        Assert.True(rightTop.Origin.DistanceTo(new Vector3d(-1.333333, 1.0, -4.9)) < 0.0001);
    }

    [Fact]
    public void Orthographic_projects_world_to_screen_by_scale()
    {
        var state = ViewProjectionState.Create(OrthographicCamera(), TestViewport(600, 600));
        var point = state.ProjectWorldPoint(new Vector3d(0.5, 0.5, 0));

        Assert.Equal(150.0, point.X, 3);
        Assert.Equal(150.0, point.Y, 3);
    }

    [Fact]
    public void Orthographic_round_trip_through_clip_space_returns_to_world()
    {
        var state = ViewProjectionState.Create(OrthographicCamera(), TestViewport(800, 600));
        var expected = new Vector3d(0.75, 1.25, 2.0);
        var clip = System.Numerics.Vector4.Transform(
            new System.Numerics.Vector4((float)expected.X, (float)expected.Y, (float)expected.Z, 1),
            state.ViewProjection);
        var actual = state.TransformPointToWorld(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);

        Assert.True(expected.DistanceTo(actual) < 0.0001);
    }

    [Fact]
    public void Orthographic_depth_range_matches_perspective_family()
    {
        var state = ViewProjectionState.Create(OrthographicCamera(), TestViewport(800, 600));

        Assert.True(state.TransformPointToWorld(0, 0, 0.0).DistanceTo(new Vector3d(0, 0, -4.9)) < 0.0001);
        Assert.True(state.TransformPointToWorld(0, 0, 1.0).DistanceTo(new Vector3d(0, 0, 95.0)) < 0.0001);
    }

    [Fact]
    public void Orthographic_projection_ignores_fov()
    {
        var wide = new CameraState(TestPosition, Vector3d.UnitZ, Vector3d.UnitY, 90, 0.1, 100, 0,
            ProjectionMode.Orthographic, 2.0);
        var stateA = ViewProjectionState.Create(OrthographicCamera(), TestViewport(800, 600));
        var stateB = ViewProjectionState.Create(wide, TestViewport(800, 600));

        Assert.Equal(stateA.ProjectWorldPoint(Vector3d.Zero), stateB.ProjectWorldPoint(Vector3d.Zero));
    }

    static CameraState OrthographicCamera() => new(TestPosition, Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0,
        ProjectionMode.Orthographic, 2.0);

    static readonly Vector3d TestPosition = new(0, 0, -5);

    static ViewportState TestViewport(int width, int height) =>
        new(0, 0, width, height, width, height, 1, 0);
}
