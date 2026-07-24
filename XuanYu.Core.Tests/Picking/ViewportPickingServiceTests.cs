using XuanYu.Core.Math;
using XuanYu.Core.Picking;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

using XuanYu.World.Scene;
namespace XuanYu.Core.Tests.Picking;

public sealed class ViewportPickingServiceTests
{
    [Fact]
    public void Center_click_hits_test_entity()
    {
        var scene = new SceneStateOwner();

        var result = Pick(scene, 400, 300);

        Assert.True(result.HasHit);
        Assert.Equal(scene.RenderSnapshot.Entity.EntityKey, result.EntityKey);
    }

    [Fact]
    public void Blank_click_returns_no_hit()
    {
        var scene = new SceneStateOwner();

        var result = Pick(scene, 780, 40);

        Assert.False(result.HasHit);
    }

    [Fact]
    public void Moved_entity_hits_new_position_not_old_position()
    {
        var scene = new SceneStateOwner();
        scene.CommitPosition(Vector3d.UnitX);

        Assert.False(Pick(scene, 400, 300).HasHit);
        Assert.True(Pick(scene, 300, 300).HasHit);
    }

    [Fact]
    public void Dpi_viewport_uses_logical_coordinates()
    {
        var scene = new SceneStateOwner();

        var result = Pick(scene, 400, 300, 1.75);

        Assert.True(result.HasHit);
        Assert.Equal(1400, Request(1, 400, 300, 1.75, scene.SpatialRevision).Viewport.PhysicalWidth);
    }

    [Fact]
    public void Rejects_stale_viewport_after_raycast()
    {
        var scene = new SceneStateOwner();
        var currentViewport = 1L;

        Assert.Throws<InvalidOperationException>(() => ViewportPickingService.Pick(
            Request(1, 400, 300, 1, scene.SpatialRevision),
            (ray, mask) => { currentViewport = 2; return scene.RaycastSpatial(ray, mask); },
            () => currentViewport,
            () => scene.SpatialRevision));
    }

    [Fact]
    public void Rejects_stale_spatial_revision_after_raycast()
    {
        var scene = new SceneStateOwner();

        Assert.Throws<InvalidOperationException>(() => ViewportPickingService.Pick(
            Request(1, 400, 300, 1, scene.SpatialRevision),
            (ray, mask) => { scene.CommitPosition(Vector3d.UnitX); return scene.RaycastSpatial(ray, mask); },
            () => 1,
            () => scene.SpatialRevision));
    }

    static ViewportPickingResult Pick(SceneStateOwner scene, double x, double y, double dpi = 1)
    {
        var request = Request(1, x, y, dpi, scene.SpatialRevision);
        return ViewportPickingService.Pick(request, scene.RaycastSpatial, () => 1, () => scene.SpatialRevision);
    }

    static ViewportPickingRequest Request(long sequence, double x, double y, double dpi, long spatialRevision) =>
        new(sequence, Viewport(dpi), Camera(1), x, y, SpatialQueryCategory.SceneEntity, spatialRevision);

    static ViewportState Viewport(double dpi) =>
        new(0, 0, 800, 600, (int)global::System.Math.Round(800 * dpi), (int)global::System.Math.Round(600 * dpi), dpi, 1);

    static CameraState Camera(long revision) =>
        new(new Vector3d(0, 0, -5), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, revision);
}
