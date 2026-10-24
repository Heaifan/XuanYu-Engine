using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

public sealed class CameraCpuFloatDepthPrecisionTests
{
    [Fact] public void CpuFloatForwardDepthNearMapsToZeroTest() =>
        Assert.Equal(0.0, CameraDepthPrecisionDiagnostic.ForwardDepth(0.05, 0.05, 80_000), 12);

    [Fact] public void CpuFloatForwardDepthFarMapsToOneTest() =>
        Assert.Equal(1.0, CameraDepthPrecisionDiagnostic.ForwardDepth(80_000, 0.05, 80_000), 12);

    [Fact] public void CpuFloatDepthQuantizationNearTest()
    {
        var sample = CameraDepthPrecisionDiagnostic.Measure(0.05, 80_000, 0.05);
        Assert.Equal(0.0, sample.DepthValue);
        Assert.True(sample.AdjacentDepth > sample.DepthValue);
        Assert.True(sample.WorldSpaceResolutionMeters > 0.0);
    }

    [Fact] public void CpuFloatDepthQuantizationFarTest()
    {
        var sample = CameraDepthPrecisionDiagnostic.Measure(80_000, 80_000, 0.05);
        Assert.Equal(1.0, sample.DepthValue);
        Assert.True(sample.AdjacentDepth < sample.DepthValue);
        Assert.True(sample.WorldSpaceResolutionMeters > 0.0);
    }

    [Fact] public void CpuFloatNear005Far80KmPrecisionTest() =>
        Assert.True(CameraDepthPrecisionDiagnostic.Measure(20_000, 80_000, 0.05)
            .WorldSpaceResolutionMeters > 1.0);

    [Fact] public void CpuFloatNear005Far400KmPrecisionTest() =>
        Assert.True(CameraDepthPrecisionDiagnostic.Measure(100_000, 400_000, 0.05)
            .WorldSpaceResolutionMeters > 1.0);

    [Fact] public void CpuFloatDynamicNearImprovesFarPrecisionTest()
    {
        var low = CameraDepthPrecisionDiagnostic.Measure(200_000, 800_000, 0.05);
        var high = CameraDepthPrecisionDiagnostic.Measure(200_000, 800_000, 100.0);
        Assert.True(high.WorldSpaceResolutionMeters < low.WorldSpaceResolutionMeters);
        Assert.True(high.PrecisionImprovementFactor > 1.0);
    }

    [Fact] public void CpuFloatFiniteDepthDiagnosticTest()
    {
        foreach (var distance in new[] { 10.0, 100, 1_000, 10_000, 20_000, 50_000, 100_000, 200_000 })
        {
            var sample = CameraDepthPrecisionDiagnostic.Measure(distance, distance * 4.0, 0.05);
            Assert.True(sample.IsFinite, $"distance={distance}");
        }
    }
}
