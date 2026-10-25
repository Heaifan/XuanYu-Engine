using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;
public sealed class ViewStateBoundaryTests
{
    [Fact]
    public void Render_origin_forces_double_precision_for_small_world_coordinate()
    {
        var camera = new CameraState(new(10001, 0, 0), -Vector3d.UnitX,
            Vector3d.UnitZ, 60, 1, 5000, 0);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 0);
        var state = ViewProjectionState.Create(camera, viewport);
        Assert.Equal(camera.Position, state.RenderOrigin);
        Assert.Equal(400, state.ProjectWorldPoint(new Vector3d(9999, 0, 0)).X, 6);
    }

    [Fact]
    public void TryCreate_rejects_invalid_camera_as_false()
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 0);
        Assert.False(ViewProjectionState.TryCreate(default, viewport, out var state));
        Assert.Null(state);
    }

    [Fact]
    public void ToWorld_rejects_non_finite_ndc()
    {
        var camera = new CameraState(Vector3d.Zero, Vector3d.UnitZ,
            Vector3d.UnitY, 60, 0.1, 100, 0);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 0);
        var state = ViewProjectionState.Create(camera, viewport);
        Assert.Throws<ArgumentOutOfRangeException>(() => state.ToWorld(double.NaN, 0, 1));
    }
}
