using XuanYu.Core.Space.Diagnostics;

namespace XuanYu.Core.Tests.Space.Diagnostics;

public sealed class TerrainSseCandidateSelectionTests
{
    [Fact]
    public void Selects_coarsest_candidate_at_each_supported_budget_boundary()
    {
        foreach (var budget in new[] { 0.5, 1d, 2d, 4d })
        {
            var result = TerrainSseCandidateSelector.Select(
                [new(0, budget / 2), new(1, budget), new(2, budget + 0.1)], budget);
            Assert.Equal(1, result.ChosenLod);
            Assert.Equal(TerrainSseErrorClassification.EstimatedWithinBudget, result.Classification);
        }
    }

    [Fact]
    public void Over_budget_candidates_choose_finest_and_remain_estimated()
    {
        var result = TerrainSseCandidateSelector.Select([new(0, 1.1), new(1, 3)], 1);
        Assert.Equal(0, result.ChosenLod);
        Assert.Equal(TerrainSseErrorClassification.EstimatedOverBudget, result.Classification);
        Assert.Equal(TerrainSseSelectionReason.AllEstimatedCandidatesOverBudget, result.Reason);
        Assert.Equal(1.1, result.EstimatedMaxPixels);
    }

    [Fact]
    public void Rejects_unsupported_budgets_and_duplicate_or_negative_lods()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TerrainSseCandidateSelector.Select([new(0, 0)], 0.75));
        Assert.Throws<ArgumentException>(() =>
            TerrainSseCandidateSelector.Select([new(0, 0), new(0, 1)], 1));
        Assert.Throws<ArgumentException>(() =>
            TerrainSseCandidateSelector.Select([new(-1, 0)], 1));
    }
}
