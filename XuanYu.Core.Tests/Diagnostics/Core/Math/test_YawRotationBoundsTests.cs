using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Foundation;

public sealed class YawRotationBoundsTests
{
    [Theory]
    [InlineData(double.MaxValue)]
    [InlineData(-double.MaxValue)]
    public void Extreme_finite_radians_produce_finite_degrees(double radians)
    {
        var rotation = YawRotation.FromRadians(radians);
        Assert.True(double.IsFinite(rotation.Degrees));
        Assert.InRange(rotation.Degrees, 0.0, 360.0);
    }

    [Fact]
    public void Quarter_turn_in_radians_rotates_x_to_y()
    {
        var rotation = YawRotation.FromRadians(global::System.Math.PI / 2.0);
        Assert.True(rotation.RotatedUnitXOnXYPlane.DistanceTo(Vector3d.UnitY) < 1e-12);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Nonfinite_radians_are_rejected(double radians)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YawRotation.FromRadians(radians));
    }
}
