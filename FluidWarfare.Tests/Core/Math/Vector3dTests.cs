using FluidWarfare.Core.Math;

namespace FluidWarfare.Tests.Core.Math;

public sealed class Vector3dTests
{
    private const double Tolerance = 1e-9;

    [Fact]
    public void Zero_ShouldHaveAllComponentsZero() => AssertVector(Vector3d.Zero, 0.0, 0.0, 0.0);

    [Fact]
    public void UnitVectors_ShouldMatchAxes()
    {
        AssertVector(Vector3d.UnitX, 1.0, 0.0, 0.0);
        AssertVector(Vector3d.UnitY, 0.0, 1.0, 0.0);
        AssertVector(Vector3d.UnitZ, 0.0, 0.0, 1.0);
    }

    [Fact]
    public void Length_ShouldUseEuclideanDistance() => Assert.Equal(5.0, new Vector3d(3.0, 4.0, 0.0).Length, Tolerance);

    [Fact]
    public void LengthSquared_ShouldAvoidSquareRoot() => Assert.Equal(25.0, new Vector3d(3.0, 4.0, 0.0).LengthSquared);

    [Fact]
    public void DistanceTo_ShouldReturnEuclideanDistance() => Assert.Equal(5.0, Vector3d.Zero.DistanceTo(new Vector3d(3.0, 4.0, 0.0)), Tolerance);

    [Fact]
    public void DistanceSquaredTo_ShouldReturnSquaredDistance() => Assert.Equal(25.0, Vector3d.Zero.DistanceSquaredTo(new Vector3d(3.0, 4.0, 0.0)));

    [Fact]
    public void Add_ShouldAddComponents() => AssertVector(new Vector3d(1.0, 2.0, 3.0) + new Vector3d(4.0, 5.0, 6.0), 5.0, 7.0, 9.0);

    [Fact]
    public void Subtract_ShouldSubtractComponents() => AssertVector(new Vector3d(5.0, 7.0, 9.0) - new Vector3d(1.0, 2.0, 3.0), 4.0, 5.0, 6.0);

    [Fact]
    public void Negate_ShouldNegateComponents() => AssertVector(-new Vector3d(1.0, -2.0, 3.0), -1.0, 2.0, -3.0);

    [Fact]
    public void Scale_ShouldMultiplyComponents() => AssertVector(new Vector3d(1.0, 2.0, 3.0) * 2.0, 2.0, 4.0, 6.0);

    [Fact]
    public void Divide_ShouldDivideComponents() => AssertVector(new Vector3d(2.0, 4.0, 6.0) / 2.0, 1.0, 2.0, 3.0);

    [Fact]
    public void Divide_ByZero_ShouldThrow() => Assert.Throws<DivideByZeroException>(() => Vector3d.UnitX / 0.0);

    [Fact]
    public void Dot_ShouldReturnDotProduct() => Assert.Equal(32.0, new Vector3d(1.0, 2.0, 3.0).Dot(new Vector3d(4.0, 5.0, 6.0)));

    [Fact]
    public void Normalize_WithNonZeroVector_ShouldReturnUnitVector() => AssertVector(new Vector3d(3.0, 4.0, 0.0).Normalize(), 0.6, 0.8, 0.0);

    [Fact]
    public void Normalize_WithZeroVector_ShouldReturnZero() => Assert.Equal(Vector3d.Zero, Vector3d.Zero.Normalize());

    [Fact]
    public void SameValue_ShouldBeEqual() => Assert.Equal(new Vector3d(1.0, 2.0, 3.0), new Vector3d(1.0, 2.0, 3.0));

    [Fact]
    public void DifferentValue_ShouldNotBeEqual() => Assert.NotEqual(new Vector3d(1.0, 2.0, 3.0), new Vector3d(3.0, 2.0, 1.0));

    [Fact]
    public void ToString_ShouldBeStable() => Assert.Equal("Vector3d(1.5, 0, -2.25)", new Vector3d(1.5, 0.0, -2.25).ToString());

    private static void AssertVector(Vector3d vector, double x, double y, double z)
    {
        Assert.Equal(x, vector.X, Tolerance);
        Assert.Equal(y, vector.Y, Tolerance);
        Assert.Equal(z, vector.Z, Tolerance);
    }
}
