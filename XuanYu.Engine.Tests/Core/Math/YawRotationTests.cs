using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Tests.Core.Math;

public sealed class YawRotationTests
{
    private const double Tolerance = 1e-9;

    [Fact]
    public void FromDegrees_ShouldNormalizePositiveOverflow() => Assert.Equal(10.0, YawRotation.FromDegrees(370.0).Degrees, Tolerance);

    [Fact]
    public void FromDegrees_ShouldNormalizeNegativeAngle() => Assert.Equal(270.0, YawRotation.FromDegrees(-90.0).Degrees, Tolerance);

    [Fact]
    public void FromDegrees_ShouldNormalizeFullTurns() => Assert.Equal(0.0, YawRotation.FromDegrees(720.0).Degrees, Tolerance);

    [Fact]
    public void FromDegrees_WithNaN_ShouldThrow() => Assert.Throws<ArgumentOutOfRangeException>(() => YawRotation.FromDegrees(double.NaN));

    [Fact]
    public void FromDegrees_WithInfinity_ShouldThrow() => Assert.Throws<ArgumentOutOfRangeException>(() => YawRotation.FromDegrees(double.PositiveInfinity));

    [Fact]
    public void FromRadians_ShouldConvertToDegrees() => Assert.Equal(90.0, YawRotation.FromRadians(global::System.Math.PI / 2.0).Degrees, Tolerance);

    [Fact]
    public void Radians_ShouldMatchDegrees() => Assert.Equal(global::System.Math.PI, YawRotation.FromDegrees(180.0).Radians, Tolerance);

    [Fact]
    public void ForwardOnXZPlane_AtZeroDegrees_ShouldFacePositiveZ() => AssertVector(YawRotation.FromDegrees(0.0).ForwardOnXZPlane, 0.0, 0.0, 1.0);

    [Fact]
    public void ForwardOnXZPlane_AtNinetyDegrees_ShouldFacePositiveX() => AssertVector(YawRotation.FromDegrees(90.0).ForwardOnXZPlane, 1.0, 0.0, 0.0);

    [Fact]
    public void ForwardOnXZPlane_AtOneEightyDegrees_ShouldFaceNegativeZ() => AssertVector(YawRotation.FromDegrees(180.0).ForwardOnXZPlane, 0.0, 0.0, -1.0);

    [Fact]
    public void ForwardOnXZPlane_AtTwoSeventyDegrees_ShouldFaceNegativeX() => AssertVector(YawRotation.FromDegrees(270.0).ForwardOnXZPlane, -1.0, 0.0, 0.0);

    [Fact]
    public void SameValue_ShouldBeEqual() => Assert.Equal(YawRotation.FromDegrees(90.0), YawRotation.FromDegrees(450.0));

    [Fact]
    public void DifferentValue_ShouldNotBeEqual() => Assert.NotEqual(YawRotation.FromDegrees(90.0), YawRotation.FromDegrees(180.0));

    [Fact]
    public void ToString_ShouldBeStable() => Assert.Equal("YawRotation(270deg)", YawRotation.FromDegrees(-90.0).ToString());

    private static void AssertVector(Vector3d vector, double x, double y, double z)
    {
        Assert.Equal(x, vector.X, Tolerance);
        Assert.Equal(y, vector.Y, Tolerance);
        Assert.Equal(z, vector.Z, Tolerance);
    }
}
