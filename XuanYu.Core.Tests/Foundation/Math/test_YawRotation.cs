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
    public void ConvertsLargeFiniteRadians()
    {
        var degrees = YawRotation.FromRadians(1e306).Degrees;

        Assert.InRange(degrees, 0.0, 359.99999999999994);
    }

    [Fact]
    public void TinyNegativeWraps()
    {
        Assert.Equal(0, YawRotation.FromDegrees(-double.Epsilon).Degrees);
    }

    [Fact]
    public void RejectsOverflow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => YawRotation.FromRadians(double.MaxValue));
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
