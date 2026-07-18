using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialRaycastRevisionTests
{
    [Fact]
    public void Scene_raycast_reports_same_revision_as_spatial_owner()
    {
        var scene = new SceneStateOwner();
        scene.CommitPosition(Vector3d.UnitX);

        var result = scene.RaycastSpatial(Ray(Vector3d.Zero, Vector3d.UnitX, 10), SpatialQueryCategory.All);

        Assert.True(result.HasHit);
        Assert.Equal(scene.SpatialRevision, result.Hit!.Value.SpatialRevision);
        Assert.Equal(scene.SpatialRevision, result.Stats.SpatialRevision);
    }

    [Fact]
    public void No_hit_still_reports_query_revision_and_stats()
    {
        var scene = new SceneStateOwner();

        var result = scene.RaycastSpatial(Ray(new Vector3d(0, 10, 0), Vector3d.UnitX, 10), SpatialQueryCategory.All);

        Assert.False(result.HasHit);
        Assert.Equal(scene.SpatialRevision, result.Stats.SpatialRevision);
        Assert.Equal(0, result.Stats.HitCount);
    }

    static SpatialRayQuery Ray(Vector3d origin, Vector3d direction, double maxDistance)
    {
        return new SpatialRayQuery(new WorldRay(origin, direction), maxDistance);
    }
}
