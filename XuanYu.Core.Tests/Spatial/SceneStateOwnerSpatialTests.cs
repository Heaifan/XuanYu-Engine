using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SceneStateOwnerSpatialTests
{
    [Fact]
    public void Initial_scene_entity_is_inserted_into_spatial_index()
    {
        var scene = new SceneStateOwner();

        var result = scene.QuerySpatial(SpatialTestData.PointQuery(0, 0), SpatialQueryCategory.SceneEntity);

        Assert.Equal(EntityId.FromInt(1), result.Candidates.Single().EntityKey);
        Assert.Equal(1, scene.SpatialRevision);
    }

    [Fact]
    public void Commit_position_updates_same_entity_without_ghost_bounds()
    {
        var scene = new SceneStateOwner();

        Assert.True(scene.CommitPosition(Vector3d.UnitX));

        Assert.Empty(scene.QuerySpatial(SpatialTestData.PointQuery(0, 0), SpatialQueryCategory.All).Candidates);
        Assert.Equal(EntityId.FromInt(1), scene.QuerySpatial(SpatialTestData.PointQuery(1, 0), SpatialQueryCategory.All).Candidates.Single().EntityKey);
        Assert.Equal(2, scene.SpatialRevision);
    }

    [Fact]
    public void Same_position_commit_does_not_change_spatial_revision()
    {
        var scene = new SceneStateOwner();

        Assert.False(scene.CommitPosition(Vector3d.Zero));

        Assert.Equal(1, scene.SpatialRevision);
    }
}
