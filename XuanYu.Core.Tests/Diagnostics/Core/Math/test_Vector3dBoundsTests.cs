using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Foundation;

public sealed class Vector3dBoundsTests
{
    [Fact]
    public void Tiny_nonzero_vector_has_nonzero_length()
    {
        var vector = new Vector3d(double.Epsilon, 0.0, 0.0);
        Assert.Equal(double.Epsilon, vector.Length);
        Assert.Equal(Vector3d.UnitX, vector.Normalize());
    }

    [Fact]
    public void Large_finite_vector_keeps_finite_length()
    {
        var half = double.MaxValue / 2.0;
        var vector = new Vector3d(half, half, 0.0);
        Assert.True(double.IsFinite(vector.Length));
        Assert.InRange(vector.Normalize().Length, 1.0 - 1e-12, 1.0 + 1e-12);
    }

    [Fact]
    public void Largest_component_can_still_be_normalized()
    {
        Assert.Equal(Vector3d.UnitX, new Vector3d(double.MaxValue, 0, 0).Normalize());
    }

    [Fact]
    public void Zero_vector_remains_zero_after_normalization()
    {
        Assert.Equal(Vector3d.Zero, Vector3d.Zero.Normalize());
    }

    [Theory]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.NaN)]
    public void Nonfinite_component_cannot_be_normalized(double x)
    {
        Assert.Throws<InvalidOperationException>(() => new Vector3d(x, 0, 0).Normalize());
    }
}
