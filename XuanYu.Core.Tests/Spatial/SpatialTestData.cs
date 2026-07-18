using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

static class SpatialTestData
{
    public static SpatialBounds Bounds(int id, double x, SpatialQueryCategory category = SpatialQueryCategory.SceneEntity)
    {
        return new SpatialBounds(EntityId.FromInt(id), Box(x, 0), category);
    }

    public static SpatialAabb Box(double x, double y)
    {
        return new SpatialAabb(new Vector3d(x - 0.2, y - 0.2, -0.2), new Vector3d(x + 0.2, y + 0.2, 0.2));
    }

    public static SpatialAabb PointQuery(double x, double y)
    {
        return new SpatialAabb(new Vector3d(x - 0.05, y - 0.05, -0.05), new Vector3d(x + 0.05, y + 0.05, 0.05));
    }

    public static SpatialIndexOwner Grid(int count)
    {
        var owner = new SpatialIndexOwner();
        for (var i = 0; i < count; i++)
        {
            var x = (i % 100) * 4.0;
            var y = (i / 100) * 4.0;
            owner.Insert(new SpatialBounds(EntityId.FromInt(i + 1), Box(x, y), SpatialQueryCategory.SceneEntity));
        }

        return owner;
    }
}
