using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Replay.Options;

public sealed class TerrainReplayOptionsTests
{
    [Theory]
    [InlineData("baseline", TerrainReplayVariant.Baseline)]
    [InlineData("far-expanded", TerrainReplayVariant.ExpandedFarPlane)]
    [InlineData("cull-off", TerrainReplayVariant.DisableChunkCulling)]
    [InlineData("lod0", TerrainReplayVariant.FixedLod0)]
    [InlineData("lod-bias-1", TerrainReplayVariant.LodBiasOne)]
    [InlineData("sse-0.5", TerrainReplayVariant.SseBudgetHalfPixel)]
    [InlineData("sse-1", TerrainReplayVariant.SseBudgetOnePixel)]
    [InlineData("sse-2", TerrainReplayVariant.SseBudgetTwoPixels)]
    [InlineData("sse-4", TerrainReplayVariant.SseBudgetFourPixels)]
    [InlineData("near-small", TerrainReplayVariant.ReducedNearPlane)]
    public void Parses_only_named_diagnostic_variants(string value,
        TerrainReplayVariant expected)
    {
        Assert.True(TerrainReplayOptions.TryParseVariant(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("1221.4", 1221.4)]
    [InlineData("0.5", 0.5)]
    public void Parses_finite_positive_far_plane_thresholds(string value, double expected)
    {
        Assert.True(TerrainReplayOptions.TryParseFarPlane(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("not-a-number")]
    public void Rejects_invalid_far_plane_thresholds(string? value)
    {
        Assert.False(TerrainReplayOptions.TryParseFarPlane(value, out _));
    }
}
