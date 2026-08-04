using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

using XuanYu.World.Scene;
namespace XuanYu.World.Tests.World;

public sealed class SceneMultiEntityGateTests
{
    [Fact]
    public void Ten_entities_project_to_render_snapshot()
    {
        var scene = new SceneStateOwner();

        scene.EnsureEntityCount(10);

        Assert.Equal(10, scene.Entities.Count);
        Assert.Equal(10, scene.RenderSnapshot.Entities.Count);
        Assert.Equal(10, scene.RenderSnapshot.Entities.Select(item => item.EntityKey).Distinct().Count());
    }

    [Fact]
    public void Picking_can_return_distinct_entity_ids()
    {
        var scene = new SceneStateOwner();
        var b = scene.CreateEntity("B", "MinimalSceneEntity", new(new Vector3d(4, 0, 0)));

        var first = scene.RaycastSpatial(Ray(-5, 0), SpatialQueryCategory.All);
        var second = scene.RaycastSpatial(Ray(4, -5, Vector3d.UnitY), SpatialQueryCategory.All);

        Assert.Equal(scene.RenderSnapshot.Entity.EntityKey, first.Hit!.Value.EntityKey);
        Assert.Equal(b.EntityKey, second.Hit!.Value.EntityKey);
    }

    [Fact]
    public void Destroyed_entity_disappears_from_snapshot_and_picking()
    {
        var scene = new SceneStateOwner();
        var doomed = scene.CreateEntity("Doomed", "MinimalSceneEntity", new(new Vector3d(4, 0, 0)));

        Assert.True(scene.DestroyEntity(doomed.EntityKey));

        Assert.DoesNotContain(scene.RenderSnapshot.Entities, item => item.EntityKey == doomed.EntityKey);
        Assert.False(scene.RaycastSpatial(Ray(4, -5, Vector3d.UnitY), SpatialQueryCategory.All).HasHit);
    }

    static SpatialRayQuery Ray(double x, double y, Vector3d? direction = null) =>
        new(new WorldRay(new Vector3d(x, y, 0), direction ?? Vector3d.UnitX), 20);
}
