using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class SpatialBoundsTests
{
    [Fact]
    public void Rejects_invalid_entity_category_and_aabb()
    {
        var box = SpatialTestData.Box(0, 0);

        Assert.Throws<ArgumentOutOfRangeException>(() => new SpatialBounds(EntityId.None, box, SpatialQueryCategory.SceneEntity));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SpatialBounds(EntityId.FromInt(1), box, SpatialQueryCategory.None));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SpatialAabb(new Vector3d(1, 0, 0), Vector3d.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SpatialAabb(new Vector3d(double.NaN, 0, 0), Vector3d.Zero));
    }

    [Fact]
    public void Intersects_and_union_are_world_space_aabb_facts()
    {
        var left = SpatialTestData.Box(0, 0);
        var right = SpatialTestData.Box(0.3, 0);
        var far = SpatialTestData.Box(10, 0);

        Assert.True(left.Intersects(right));
        Assert.False(left.Intersects(far));
        Assert.True(left.Union(far).Intersects(far));
    }
}
