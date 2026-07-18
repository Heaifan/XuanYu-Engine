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
    public void Corners_change_direction_in_expected_axes()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));
        var leftTop = WorldRayFactory.FromViewportPoint(state, 0, 0);
        var rightBottom = WorldRayFactory.FromViewportPoint(state, 800, 600);

        Assert.NotEqual(leftTop.Direction.X, rightBottom.Direction.X);
        Assert.NotEqual(leftTop.Direction.Y, rightBottom.Direction.Y);
        Assert.Equal(leftTop.Direction.Z, rightBottom.Direction.Z, precision: 6);
    }

    [Fact]
    public void Resize_uses_new_viewport_aspect()
    {
        var camera = TestCamera();
        var wide = ViewProjectionState.Create(camera, TestViewport(1200, 600));
        var narrow = ViewProjectionState.Create(camera, TestViewport(600, 600));

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
