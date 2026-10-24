using XuanYu.Core.Space.Diagnostics;

namespace XuanYu.Core.Tests.Replay.Sse;

public sealed class TerrainSseCandidateSafetyTests
{
    [Fact]
    public void Inside_camera_forces_finest_even_when_its_estimate_is_missing()
    {
        var result = TerrainSseCandidateSelector.Select([new(0, null), new(2, 0.1)], 1,
            cameraInsideChunk: true);
        Assert.Equal(0, result.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.CameraInsideChunkFallback, result.Reason);
        Assert.Equal(TerrainSseErrorClassification.EstimateUnavailable, result.Classification);
    }

    [Fact]
    public void Near_plane_forces_finest_and_keeps_over_budget_classification()
    {
        var result = TerrainSseCandidateSelector.Select([new(0, 2), new(1, 0.1)], 1,
            nearPlaneIntersecting: true);
        Assert.Equal(0, result.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.NearPlaneIntersectionFallback, result.Reason);
        Assert.Equal(TerrainSseErrorClassification.EstimatedOverBudget, result.Classification);
    }

    [Fact]
    public void Missing_or_invalid_estimates_fall_back_to_finest_with_explicit_classification()
    {
        var unavailable = TerrainSseCandidateSelector.Select([new(0, null), new(1, null)], 1);
        Assert.Equal(TerrainSseErrorClassification.EstimateUnavailable, unavailable.Classification);
        Assert.Equal(TerrainSseSelectionReason.EstimateUnavailableFallback, unavailable.Reason);

        foreach (var invalid in new[] { double.NaN, double.PositiveInfinity, -0.1 })
        {
            var result = TerrainSseCandidateSelector.Select([new(0, invalid), new(1, null)], 1);
            Assert.Equal(0, result.ChosenLod);
            Assert.Null(result.EstimatedMaxPixels);
            Assert.Equal(TerrainSseErrorClassification.InvalidEstimate, result.Classification);
            Assert.Equal(TerrainSseSelectionReason.InvalidEstimateFallback, result.Reason);
        }
    }

    [Fact]
    public void Rejects_invalid_hysteresis()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TerrainSseCandidateSelector.Select([new(0, 0)], 1, hysteresisFraction: 1));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TerrainSseCandidateSelector.Select([new(0, 0)], 1, hysteresisFraction: double.NaN));
    }

    [Fact]
    public void Incomplete_measurements_retain_previous_valid_lod_or_fall_back_to_finest()
    {
        var held = TerrainSseCandidateSelector.Select([new(0, null), new(1, 0.8), new(2, 0.2)],
            1, previousLod: 1);
        Assert.Equal(1, held.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.EstimateUnavailableFallback, held.Reason);

        var fine = TerrainSseCandidateSelector.Select([new(0, null), new(1, 0.8), new(2, 0.2)], 1);
        Assert.Equal(0, fine.ChosenLod);
        Assert.Equal(TerrainSseErrorClassification.EstimateUnavailable, fine.Classification);
    }
}
