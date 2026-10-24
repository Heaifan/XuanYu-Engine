using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Foundation.Math;

public sealed class YawRotationTests
{
    [Fact]
    public void WrapsDegrees()
    {
        Assert.Equal(270, YawRotation.FromDegrees(-90).Degrees);
        Assert.Equal(0, YawRotation.FromDegrees(360).Degrees);
    }

    [Fact]
    public void ConvertsRadians()
    {
        var result = YawRotation.FromRadians(global::System.Math.PI);

        Assert.Equal(180, result.Degrees, 10);
    }

    [Fact]
    public void RotatesXYBasis()
    {
        var rotation = YawRotation.FromDegrees(90);
        var x = rotation.RotatedUnitXOnXYPlane;

        Assert.Equal(0, x.X, 10);
        Assert.Equal(1, x.Y, 10);
        Assert.Equal(0, x.Z);
    }

    [Fact]
    public void RejectsNonFinite()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => YawRotation.FromDegrees(double.PositiveInfinity));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => YawRotation.FromRadians(double.NaN));
    }
}
