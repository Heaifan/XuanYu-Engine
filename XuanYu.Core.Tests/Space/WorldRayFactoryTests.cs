using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class WorldRayFactoryTests
{
    [Fact]
    public void Center_viewport_point_matches_camera_forward()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));
        var ray = WorldRayFactory.FromViewportPoint(state, 400, 300);

        SpaceAssert.Near(Vector3d.UnitZ, ray.Direction);
        SpaceAssert.Near(new Vector3d(0, 0, -4.9), ray.Origin);
    }

    [Fact]
    public void Four_corners_freeze_screen_to_world_axis_convention()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));
        var leftTop = WorldRayFactory.FromViewportPoint(state, 0, 0);
        var rightTop = WorldRayFactory.FromViewportPoint(state, 800, 0);
        var leftBottom = WorldRayFactory.FromViewportPoint(state, 0, 600);
        var rightBottom = WorldRayFactory.FromViewportPoint(state, 800, 600);

        Assert.True(leftTop.Direction.X > 0.0);
        Assert.True(rightTop.Direction.X < 0.0);
        Assert.True(leftTop.Direction.Y > 0.0);
        Assert.True(leftBottom.Direction.Y < 0.0);
        Assert.True(leftTop.Direction.Z > 0.0);
        Assert.True(rightBottom.Direction.Z > 0.0);
    }

    [Fact]
    public void Non_zero_viewport_origin_uses_local_center()
    {
        var state = ViewProjectionState.Create(
            TestCamera(),
            new ViewportState(200, 100, 800, 600, 800, 600, 1, 0));

        var ray = WorldRayFactory.FromViewportPoint(state, 600, 400);

        SpaceAssert.Near(Vector3d.UnitZ, ray.Direction);
    }

    [Fact]
    public void Resize_keeps_center_ray_and_changes_edge_aspect()
    {
        var camera = TestCamera();
        var normal = ViewProjectionState.Create(camera, TestViewport(800, 600));
        var wide = ViewProjectionState.Create(camera, TestViewport(1200, 600));
        var narrow = ViewProjectionState.Create(camera, TestViewport(600, 600));

        SpaceAssert.Near(Vector3d.UnitZ, WorldRayFactory.FromViewportPoint(normal, 400, 300).Direction);
        SpaceAssert.Near(Vector3d.UnitZ, WorldRayFactory.FromViewportPoint(wide, 600, 300).Direction);
        var wideRay = WorldRayFactory.FromViewportPoint(wide, 0, 300);
        var narrowRay = WorldRayFactory.FromViewportPoint(narrow, 0, 300);

        Assert.True(global::System.Math.Abs(wideRay.Direction.X) > global::System.Math.Abs(narrowRay.Direction.X));
    }

    [Fact]
    public void Same_input_returns_stable_ray_and_rejects_invalid_point()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));

        Assert.Equal(WorldRayFactory.FromViewportPoint(state, 400, 300), WorldRayFactory.FromViewportPoint(state, 400, 300));
        Assert.Throws<ArgumentOutOfRangeException>(() => WorldRayFactory.FromViewportPoint(state, double.NaN, 300));
    }

    static CameraState TestCamera()
    {
        return new CameraState(new Vector3d(0, 0, -5), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0);
    }

    static ViewportState TestViewport(int width, int height)
    {
        return new ViewportState(0, 0, width, height, width, height, 1, 0);
    }
}
