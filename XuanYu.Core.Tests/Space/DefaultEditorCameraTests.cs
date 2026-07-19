using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class DefaultEditorCameraTests
{
    [Fact]
    public void Forward_is_derived_from_position_to_target()
    {
        var camera = DefaultEditorCamera.Create(7);
        var expected = (DefaultEditorCamera.Target - DefaultEditorCamera.Position).Normalize();

        SpaceAssert.Near(expected, camera.Forward);
        Assert.Equal(7, camera.Revision);
    }

    [Fact]
    public void Center_ray_points_at_target_after_resize()
    {
        var camera = DefaultEditorCamera.Create(1);
        var normal = State(camera, 800, 600);
        var wide = State(camera, 1200, 600);

        Assert.True(camera.Forward.Dot(WorldRayFactory.FromViewportPoint(normal, 400, 300).Direction) > 0.99999999);
        Assert.True(camera.Forward.Dot(WorldRayFactory.FromViewportPoint(wide, 600, 300).Direction) > 0.99999999);
    }

    static ViewProjectionState State(CameraState camera, int width, int height) =>
        ViewProjectionState.Create(camera, new ViewportState(0, 0, width, height, width, height, 1, 1));
}
