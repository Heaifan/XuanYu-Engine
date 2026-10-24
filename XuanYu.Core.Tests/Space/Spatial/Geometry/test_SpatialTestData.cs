using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Spatial;

static class SpatialTestData
{
    public static SpatialAabb Box(double x, double y)
    {
        return new SpatialAabb(new Vector3d(x - 0.2, y - 0.2, -0.2), new Vector3d(x + 0.2, y + 0.2, 0.2));
    }

    public static SpatialAabb PointQuery(double x, double y)
    {
        return new SpatialAabb(new Vector3d(x - 0.05, y - 0.05, -0.05), new Vector3d(x + 0.05, y + 0.05, 0.05));
    }
}
