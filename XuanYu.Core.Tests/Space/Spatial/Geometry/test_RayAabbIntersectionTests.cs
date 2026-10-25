using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Tests.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

public sealed class RayAabbIntersectionTests
{
    [Fact]
    public void Hits_front_face_inside_origin_edge_and_corner()
    {
        var box = SpatialTestData.Box(4, 0);

        AssertHit(Ray(Vector3d.Zero, Vector3d.UnitX, 10), box, 3.8);
        AssertHit(Ray(new Vector3d(4, 0, 0), Vector3d.UnitX, 10), box, 0.0);
        AssertHit(Ray(new Vector3d(0, 0.2, 0), Vector3d.UnitX, 10), box, 3.8);
        AssertHit(Ray(new Vector3d(0, 0.2, 0.2), Vector3d.UnitX, 10), box, 3.8);
    }

    [Fact]
    public void Rejects_miss_back_facing_negative_only_and_beyond_max_distance()
    {
        var box = SpatialTestData.Box(4, 0);

        Assert.False(RayAabbIntersection.TryHit(Ray(new Vector3d(0, 3, 0), Vector3d.UnitX, 10), box, out _));
        Assert.False(RayAabbIntersection.TryHit(Ray(Vector3d.Zero, -Vector3d.UnitX, 10), box, out _));
        Assert.False(RayAabbIntersection.TryHit(Ray(new Vector3d(5, 0, 0), Vector3d.UnitX, 10), box, out _));
        Assert.False(RayAabbIntersection.TryHit(Ray(Vector3d.Zero, Vector3d.UnitX, 3.7), box, out _));
    }

    [Fact]
    public void Handles_parallel_axes_and_max_distance_boundary()
    {
        var box = SpatialTestData.Box(0, 4);

        AssertHit(Ray(Vector3d.Zero, Vector3d.UnitY, 3.8), box, 3.8);
        Assert.False(RayAabbIntersection.TryHit(Ray(new Vector3d(1, 0, 0), Vector3d.UnitY, 10), box, out _));
    }

    [Fact]
    public void Tiny_nonzero_axis_still_intersects_at_long_distance()
    {
        var box = new SpatialAabb(new(1e9, .09, -1), new(1e9 + 10, .11, 1));
        var ray = Ray(Vector3d.Zero, new(1, 1e-10, 0), 2e9);
        Assert.True(RayAabbIntersection.TryHit(ray, box, out _));
        Assert.True(SpatialRayAabb.Intersects(ray, box));
    }

    static SpatialRayQuery Ray(Vector3d origin, Vector3d direction, double maxDistance)
    {
        return new SpatialRayQuery(new WorldRay(origin, direction), maxDistance);
    }

    static void AssertHit(SpatialRayQuery ray, SpatialAabb box, double distance)
    {
        Assert.True(RayAabbIntersection.TryHit(ray, box, out var hit));
        SpaceAssert.Near(distance, hit.Distance);
    }
}
