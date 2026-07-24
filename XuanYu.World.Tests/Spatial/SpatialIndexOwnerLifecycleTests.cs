using XuanYu.Core.Identity;
using XuanYu.Core.Spatial;

using XuanYu.World.Spatial;
namespace XuanYu.World.Tests.Spatial;

public sealed class SpatialIndexOwnerLifecycleTests
{
    [Fact]
    public void Insert_remove_and_category_mask_control_candidates()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 0));
        owner.Insert(SpatialTestData.Bounds(2, 4, SpatialQueryCategory.Gizmo));

        var scene = owner.Query(SpatialTestData.PointQuery(0, 0), SpatialQueryCategory.SceneEntity);
        var gizmo = owner.Query(SpatialTestData.PointQuery(4, 0), SpatialQueryCategory.SceneEntity);

        Assert.Equal(EntityId.FromInt(1), scene.Candidates.Single().EntityKey);
        Assert.Empty(gizmo.Candidates);
        Assert.True(owner.Remove(EntityId.FromInt(1)));
        Assert.Empty(owner.Query(SpatialTestData.PointQuery(0, 0), SpatialQueryCategory.All).Candidates);
    }

    [Fact]
    public void Update_moves_entity_without_ghost_candidate()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 0));

        Assert.True(owner.Update(SpatialTestData.Bounds(1, 8)));

        Assert.Empty(owner.Query(SpatialTestData.PointQuery(0, 0), SpatialQueryCategory.All).Candidates);
        Assert.Equal(EntityId.FromInt(1), owner.Query(SpatialTestData.PointQuery(8, 0), SpatialQueryCategory.All).Candidates.Single().EntityKey);
    }

    [Fact]
    public void Duplicate_and_missing_lifecycle_actions_are_explicit()
    {
        var owner = new SpatialIndexOwner();
        owner.Insert(SpatialTestData.Bounds(1, 0));

        Assert.Throws<InvalidOperationException>(() => owner.Insert(SpatialTestData.Bounds(1, 0)));
        Assert.False(owner.Remove(EntityId.FromInt(2)));
        Assert.Throws<KeyNotFoundException>(() => owner.Update(SpatialTestData.Bounds(2, 0)));
    }
}
