using System.Linq;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World;

namespace XuanYu.World.Tests.World;

// R2-R1 final patch: lock the two spatial-bounds semantics so a future change cannot
// silently restore a "global default box" or a point/box confusion. WorldQuery must
// consume bounds, never invent them.
public sealed class EntityBoundsSemanticsTests
{
    // Default extent (no explicit extent) means a ZERO-SIZE POINT at the entity position.
    // It is NOT "no spatial info" and NOT a default unit box -- World never invents size.
    [Fact]
    public void Default_extent_is_zero_size_point_bounds()
    {
        var world = new GlobalWorld();
        var e = world.Create("Point", transform: new CommittedTransform(new Vector3d(10, 0, 0)));

        Assert.Equal(new Vector3d(10, 0, 0), e.Bounds.WorldBounds.Min);
        Assert.Equal(new Vector3d(10, 0, 0), e.Bounds.WorldBounds.Max);

        var keys = world.QuerySpatial(Area(10, 0), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey);
        Assert.Contains(e.EntityKey, keys);

        var offset = world.QuerySpatial(Area(10, 0.55), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey);
        Assert.DoesNotContain(e.EntityKey, offset);
    }

    // Explicit extent is the entity's OWN spatial description; it must produce the correct
    // absolute world bounds and be hittable as a real box (a plain point would miss here).
    [Fact]
    public void Explicit_extent_is_absolute_world_bounds()
    {
        var world = new GlobalWorld();
        var extent = new SpatialAabb(new Vector3d(-0.5, -0.5, -0.5), new Vector3d(0.5, 0.5, 0.5));
        var e = world.Create("Boxed", transform: new CommittedTransform(new Vector3d(10, 0, 0)), extent: extent);

        Assert.Equal(new Vector3d(9.5, -0.5, -0.5), e.Bounds.WorldBounds.Min);
        Assert.Equal(new Vector3d(10.5, 0.5, 0.5), e.Bounds.WorldBounds.Max);

        var inside = world.QuerySpatial(Area(10, 0.55), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey);
        Assert.Contains(e.EntityKey, inside);

        var outside = world.QuerySpatial(Area(10, 1.2), SpatialQueryCategory.All).Candidates.Select(c => c.EntityKey);
        Assert.DoesNotContain(e.EntityKey, outside);
    }

    static SpatialAabb Area(double x, double y) =>
        new(new Vector3d(x - 0.5, y - 0.5, -0.5), new Vector3d(x + 0.5, y + 0.5, 0.5));
}
