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
    public void World_point_round_trip_through_clip_space_returns_to_world()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 600));
        var expected = new Vector3d(0.75, 1.25, 2.0);
        var clip = Vector4.Transform(new Vector4((float)expected.X, (float)expected.Y, (float)expected.Z, 1), state.ViewProjection);

        var actual = state.TransformPointToWorld(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);

        Assert.True(expected.DistanceTo(actual) < 0.0001);
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

    static CameraState TestCamera()
    {
        return new CameraState(new Vector3d(0, 0, -5), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 0);
    }

    static ViewportState TestViewport(int width, int height)
    {
        return new ViewportState(0, 0, width, height, width, height, 1, 0);
    }
}
