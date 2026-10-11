using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class SceneFarPlanePolicyTests
{
    [Fact]
    public void Scene_bounds_are_covered_when_terrain_is_empty()
    {
        var camera = new CameraState(Vector3d.Zero, Vector3d.UnitX, Vector3d.UnitZ,
            60, 0.1, 700, 1);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var modelBounds = new SpatialAabb(new(990, -5, -5), new(1010, 5, 5));

        var far = TerrainFarPlanePolicy.Resolve(camera, viewport, [], [modelBounds]);

        Assert.True(far >= 1060);
    }
}
