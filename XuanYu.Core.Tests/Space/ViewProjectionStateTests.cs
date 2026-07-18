using System.Numerics;
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
    public void Projection_has_expected_aspect_and_is_invertible()
    {
        var state = ViewProjectionState.Create(TestCamera(), TestViewport(800, 400));

        Assert.True(state.Projection.M11 > 0.0f);
        Assert.True(state.Projection.M22 > state.Projection.M11);
        Assert.True(Matrix4x4.Invert(state.ViewProjection, out _));
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
