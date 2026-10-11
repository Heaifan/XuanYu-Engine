using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class TerrainFarPlaneOrthographicTests
{
    [Fact]
    public void Parallel_side_planes_reject_bounds_outside_orthographic_width()
    {
        var camera = new CameraState(Vector3d.Zero, Vector3d.UnitX, Vector3d.UnitZ,
            60, 0.1, 700, 1, ProjectionMode.Orthographic, 1000);
        var viewport = new ViewportState(0, 0, 1600, 800, 1600, 800, 1, 1);
        var outside = new SpatialAabb(new(990, 1490, -10), new(1010, 1510, 10));

        var far = TerrainFarPlanePolicy.Resolve(camera, viewport, [outside]);

        Assert.Equal(camera.FarPlane, far);
    }

    [Fact]
    public void Orthographic_bounds_inside_parallel_side_planes_extend_far()
    {
        var camera = new CameraState(Vector3d.Zero, Vector3d.UnitX, Vector3d.UnitZ,
            60, 0.1, 700, 1, ProjectionMode.Orthographic, 1000);
        var viewport = new ViewportState(0, 0, 1600, 800, 1600, 800, 1, 1);
        var visible = new SpatialAabb(new(2990, -10, -10), new(3010, 10, 10));

        Assert.True(TerrainFarPlanePolicy.Resolve(camera, viewport, [visible]) > 3000);
    }
}
