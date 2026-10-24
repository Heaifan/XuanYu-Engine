using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class WorldRayTests
{
    [Fact]
    public void RejectsInvalidDirection()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new WorldRay(Vector3d.Zero, Vector3d.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => new WorldRay(Vector3d.Zero, new Vector3d(double.NaN, 0, 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new WorldRay(Vector3d.Zero, new Vector3d(0, double.PositiveInfinity, 1)));
    }
}
