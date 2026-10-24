using XuanYu.Core.Space.Diagnostics;

namespace XuanYu.Core.Tests.Replay.Sse;

public sealed class TerrainSseCandidateHysteresisTests
{
    [Fact]
    public void Coarsens_only_when_candidate_is_below_lower_hysteresis_threshold()
    {
        var measurements = new[] { new TerrainSseLodMeasurement(0, 0.8), new(1, 0.9), new(2, 0.85) };
        var held = TerrainSseCandidateSelector.Select(measurements, 1, 0, 0.2);
        Assert.Equal(0, held.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.HysteresisRetainedPreviousLod, held.Reason);

        var switched = TerrainSseCandidateSelector.Select(
            [new(0, 0.8), new(1, 0.6), new(2, 0.5)], 1, 0, 0.2);
        Assert.Equal(2, switched.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.CoarsestWithinHysteresisThreshold, switched.Reason);
    }

    [Fact]
    public void Refines_only_after_previous_estimate_crosses_upper_hysteresis_threshold()
    {
        var measurements = new[] { new TerrainSseLodMeasurement(0, 0.4), new(1, 1.1), new(2, 1.19) };
        var held = TerrainSseCandidateSelector.Select(measurements, 1, 1, 0.2);
        Assert.Equal(1, held.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.HysteresisRetainedPreviousLod, held.Reason);
        Assert.Equal(TerrainSseErrorClassification.EstimatedOverBudget, held.Classification);

        var refined = TerrainSseCandidateSelector.Select(
            [new(0, 0.4), new(1, 1.21), new(2, 1.3)], 1, 1, 0.2);
        Assert.Equal(0, refined.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.CoarsestWithinBudget, refined.Reason);
    }

    [Fact]
    public void Missing_previous_measurement_does_not_apply_hysteresis()
    {
        var result = TerrainSseCandidateSelector.Select([new(0, 0.4), new(2, 0.8)], 1, 1, 0.2);
        Assert.Equal(2, result.ChosenLod);
        Assert.Equal(TerrainSseSelectionReason.CoarsestWithinBudget, result.Reason);
    }
}
