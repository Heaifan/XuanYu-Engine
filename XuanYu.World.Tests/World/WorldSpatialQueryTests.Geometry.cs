using XuanYu.Core.Math;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldSpatialQueryTests
{
    static double DistanceSquared(Vector3d a, Vector3d b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }
}
