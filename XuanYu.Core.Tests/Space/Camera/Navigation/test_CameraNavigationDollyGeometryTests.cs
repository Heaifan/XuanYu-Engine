using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

public sealed class CameraNavigationDollyGeometryTests
{
    [Fact]
    public void Dolly_translation_stays_on_original_forward_when_pivot_is_off_ray()
    {
        var camera = DefaultEditorCamera.Create(1);
        var pivot = new Vector3d(3, 2, 0);
        Assert.True(CameraNavigation.TryDolly(camera, pivot, 1, 2, out var result, out var reason), reason);
        var delta = result.Camera.Position - camera.Position;
        var direction = delta.Normalize();
        Assert.InRange(direction.Dot(camera.Forward), 1.0 - 1e-9, 1.0 + 1e-9);
        Assert.InRange(direction.Cross(camera.Forward).Length, 0.0, 1e-9);
        AssertVector(camera.Forward, result.Camera.Forward);
        AssertVector(camera.Up, result.Camera.Up);
        Assert.Equal(camera.VerticalFovDegrees, result.Camera.VerticalFovDegrees);
    }

    [Fact]
    public void Dolly_keeps_focused_world_point_screen_position_stable()
    {
        var pivot = Vector3d.Zero;
        var camera = DefaultEditorCamera.Create(1);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var expected = ViewProjectionState.Create(camera, viewport).ProjectWorldPoint(pivot);
        for (var i = 0; i < 20; i++)
        {
            Assert.True(CameraNavigation.TryDolly(camera, pivot, 1, i + 2, out var next, out _));
            camera = next.Camera;
        }
        for (var i = 0; i < 20; i++)
        {
            Assert.True(CameraNavigation.TryDolly(camera, pivot, -1, i + 22, out var next, out _));
            camera = next.Camera;
        }
        var actual = ViewProjectionState.Create(camera, viewport).ProjectWorldPoint(pivot);
        Assert.Equal(expected.X, actual.X, 6);
        Assert.Equal(expected.Y, actual.Y, 6);
    }

    static void AssertVector(Vector3d expected, Vector3d actual)
    {
        Assert.Equal(expected.X, actual.X, 12);
        Assert.Equal(expected.Y, actual.Y, 12);
        Assert.Equal(expected.Z, actual.Z, 12);
    }
}
