using System.Numerics;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class ViewProjectionStateTests
{
    [Fact]
    public void Creates_known_view_matrix_for_camera_looking_forward()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));

        SpaceAssert.Near(0.0, state.View.M41);
        SpaceAssert.Near(0.0, state.View.M42);
        SpaceAssert.Near(-5.0, state.View.M43);
    }

    [Fact]
    public void Projection_is_canonical_right_handed_and_is_invertible()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 400));

        Assert.True(state.Projection.M11 > 0.0f);
        Assert.True(state.Projection.M22 > state.Projection.M11);
        Assert.True(Matrix4x4.Invert(state.ViewProjection, out _));
    }

    [Fact]
    public void Camera_up_projects_toward_screen_top()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));
        var origin = state.ProjectWorldPoint(Vector3d.Zero);
        var up = state.ProjectWorldPoint(state.Camera.Up);

        Assert.True(up.Y < origin.Y);
    }

    [Fact]
    public void World_point_projection_matches_known_perspective_screen_and_depth()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));
        var expected = new Vector3d(0.0, 0.0, 5.0);

        Assert.Equal(new ScreenPoint(400, 300), state.ProjectWorldPoint(expected));
        var near = state.TransformPointToWorld(0, 0, 1);
        var far = state.TransformPointToWorld(0, 0, 0);
        Assert.True(near.DistanceTo(new Vector3d(0, 0, -4.9)) < 0.0001);
        Assert.True(far.DistanceTo(new Vector3d(0, 0, 95)) < 0.0001);
    }

    [Fact]
    public void Try_project_returns_true_for_front_point()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));

        Assert.True(state.TryProjectWorldPoint(Vector3d.Zero, out var screen));
        Assert.Equal(new ScreenPoint(400, 300), screen);
    }

    [Fact]
    public void Try_project_returns_false_for_behind_point_and_strict_api_throws()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));

        Assert.False(state.TryProjectWorldPoint(new Vector3d(0, 0, -6), out _));
        Assert.Throws<InvalidOperationException>(() => state.ProjectWorldPoint(new Vector3d(0, 0, -6)));
    }

    [Fact]
    public void Large_world_projection_preserves_known_camera_relative_coordinates()
    {
        var camera = new CameraState(new(1_000_000_000.25, -1_000_000_000.5, 300_000_000.75), Vector3d.UnitY,
            Vector3d.UnitZ, 60, 1, 500000, 0);
        var state = ViewProjectionState.Create(camera, TestViewport(800, 600));

        Assert.Equal(camera.Position, state.RenderOrigin);
        Assert.True(MathF.Abs(state.View.M41) < 0.0001f);
        Assert.True(MathF.Abs(state.View.M42) < 0.0001f);
        Assert.True(MathF.Abs(state.View.M43) < 0.0001f);
        var center = camera.Position + camera.Forward * 1000;
        var right = center + camera.Right * (1000 * global::System.Math.Tan(global::System.Math.PI / 6) * 800.0 / 600.0);
        Assert.Equal(new ScreenPoint(400, 300), state.ProjectWorldPoint(center));
        var rightScreen = state.ProjectWorldPoint(right);
        Assert.InRange(rightScreen.X, 799.999, 800.001);
        Assert.Equal(300, rightScreen.Y, 6);
        var relative = center - state.RenderOrigin;
        Assert.True(relative.DistanceTo(new Vector3d(0, 1000, 0)) < 0.000001);
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
