using Xunit;

namespace XuanYu.Core.Tests.Camera;

public sealed class ReverseZProjectionPrototypeTests
{
    const double Near = 0.05;
    static readonly double[] Distances = [1_000, 10_000, 20_000, 50_000, 100_000, 200_000, 500_000];

    [Fact] public void ReversePerspectiveNearMapsToOneTest() => Assert.Equal(1, Perspective(Near, Near, 500_000, true), 12);
    [Fact] public void ReversePerspectiveFarMapsToZeroTest() => Assert.Equal(0, Perspective(500_000, Near, 500_000, true), 12);
    [Fact] public void ReverseOrthographicNearMapsToOneTest() => Assert.Equal(1, Ortho(Near, Near, 500_000, true), 12);
    [Fact] public void ReverseOrthographicFarMapsToZeroTest() => Assert.Equal(0, Ortho(500_000, Near, 500_000, true), 12);

    [Fact]
    public void ReverseDepthMonotonicityTest()
    {
        for (var i = 1; i < Distances.Length; i++)
            Assert.True(Perspective(Distances[i], Near, 500_000, true) < Perspective(Distances[i - 1], Near, 500_000, true));
    }

    [Fact]
    public void ReverseD32FarPrecisionBeatsForwardTest()
    {
        foreach (var distance in Distances)
        {
            var far = global::System.Math.Max(1_000, distance * 4);
            Assert.True(Resolution(distance, Near, far, true) < Resolution(distance, Near, far, false), $"distance={distance}");
        }
    }

    [Fact]
    public void ReverseProjectionInverseRoundTripTest()
    {
        foreach (var distance in Distances)
        {
            var depth = Perspective(distance, Near, 500_000, true);
            Assert.Equal(distance, Inverse(depth, Near, 500_000, true), 7);
        }
    }

    [Fact]
    public void VulkanZeroToOneContractTest()
    {
        Assert.InRange(Perspective(Near, Near, 500_000, true), 0, 1);
        Assert.InRange(Perspective(500_000, Near, 500_000, true), 0, 1);
        Assert.InRange(Ortho(Near, Near, 500_000, true), 0, 1);
        Assert.InRange(Ortho(500_000, Near, 500_000, true), 0, 1);
    }

    static double Perspective(double z, double n, double f, bool reverse) => reverse
        ? (f * n / z - n) / (f - n)
        : (f - f * n / z) / (f - n);
    static double Ortho(double z, double n, double f, bool reverse) => reverse
        ? (f - z) / (f - n) : (z - n) / (f - n);
    static double Inverse(double depth, double n, double f, bool reverse) => reverse
        ? f * n / (n + depth * (f - n))
        : f * n / (f - depth * (f - n));
    static double Resolution(double z, double n, double f, bool reverse)
    {
        var depth = (float)Perspective(z, n, f, reverse);
        var adjacent = reverse ? float.BitDecrement(depth) : float.BitIncrement(depth);
        return global::System.Math.Abs(Inverse(adjacent, n, f, reverse) - z);
    }
}
