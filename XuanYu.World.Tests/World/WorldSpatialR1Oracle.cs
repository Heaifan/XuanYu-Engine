using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.World;

namespace XuanYu.World.Tests.World;

static class WorldSpatialR1Oracle
{
    public static IReadOnlyList<EntityId> Bounds(GlobalWorld world, SpatialAabb bounds) =>
        world.Entities
            .Where(e => bounds.Intersects(EntityBox(e.GlobalPosition)))
            .Select(e => e.EntityKey)
            .OrderBy(id => id.Value)
            .ToArray();

    public static IReadOnlyList<EntityId> Radius(GlobalWorld world, Vector3d center, double radius) =>
        world.Entities
            .Where(e => DistanceSquared(e.GlobalPosition, center) <= radius * radius)
            .Select(e => e.EntityKey)
            .OrderBy(id => id.Value)
            .ToArray();

    public static SpatialAabb BoundsAround(Vector3d center, double half) =>
        new(
            new Vector3d(center.X - half, center.Y - half, center.Z - half),
            new Vector3d(center.X + half, center.Y + half, center.Z + half));

    public static void AssertSame(IReadOnlyList<EntityId> expected, IReadOnlyList<EntityId> actual) =>
        Assert.Equal(expected, actual.OrderBy(id => id.Value));

    // Entity half-extent matches WorldQuery.PointBounds (R2 single-authority box).
    static SpatialAabb EntityBox(Vector3d p) =>
        new(new Vector3d(p.X - 0.5, p.Y - 0.5, p.Z - 0.5), new Vector3d(p.X + 0.5, p.Y + 0.5, p.Z + 0.5));

    static double DistanceSquared(Vector3d a, Vector3d b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }
}
