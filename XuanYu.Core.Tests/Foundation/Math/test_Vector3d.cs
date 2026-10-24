using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Foundation.Math;

public sealed class Vector3dTests
{
    [Fact]
    public void AddsAndSubtracts()
    {
        var first = new Vector3d(1, 2, 3);
        var second = new Vector3d(4, 5, 6);

        Assert.Equal(new Vector3d(5, 7, 9), first + second);
        Assert.Equal(new Vector3d(-3, -3, -3), first - second);
    }

    [Fact]
    public void DotAndCross()
    {
        Assert.Equal(0, Vector3d.UnitX.Dot(Vector3d.UnitY));
        Assert.Equal(Vector3d.UnitZ, Vector3d.UnitX.Cross(Vector3d.UnitY));
    }

    [Fact]
    public void NormalizesZero()
    {
        Assert.Equal(Vector3d.Zero, Vector3d.Zero.Normalize());
        Assert.Equal(1.0, new Vector3d(3, 4, 0).Normalize().Length, 12);
    }

    [Fact]
    public void DivisionRejectsZero()
    {
        Assert.Throws<DivideByZeroException>(() => Vector3d.UnitX / 0.0);
    }
}
