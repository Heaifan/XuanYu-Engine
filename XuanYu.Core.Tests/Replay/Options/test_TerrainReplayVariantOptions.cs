using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Replay.Options;

public sealed class TerrainReplayVariantOptionsTests
{
    [Fact]
    public void Culling_and_lod_variants_change_only_requested_decisions()
    {
        Assert.False(TerrainReplayOptions.ShouldCull(false,
            TerrainReplayVariant.DisableChunkCulling));
        Assert.True(TerrainReplayOptions.ShouldCull(false,
            TerrainReplayVariant.Baseline));
        Assert.Equal(0, TerrainReplayOptions.SelectLod(3,
            TerrainReplayVariant.FixedLod0));
        Assert.Equal(3, TerrainReplayOptions.SelectLod(3,
            TerrainReplayVariant.Baseline));
        Assert.Equal(4, TerrainReplayOptions.SelectLod(3,
            TerrainReplayVariant.LodBiasOne));
        Assert.Equal(1, TerrainReplayOptions.SelectLod(0,
            TerrainReplayVariant.LodBiasOne));
    }

    [Fact]
    public void Lod_bias_does_not_wrap_large_integer()
    {
        Assert.Equal(4, TerrainReplayOptions.SelectLod(
            int.MaxValue, TerrainReplayVariant.LodBiasOne));
    }

    [Theory]
    [InlineData(TerrainReplayVariant.SseBudgetHalfPixel, 0.5)]
    [InlineData(TerrainReplayVariant.SseBudgetOnePixel, 1.0)]
    [InlineData(TerrainReplayVariant.SseBudgetTwoPixels, 2.0)]
    [InlineData(TerrainReplayVariant.SseBudgetFourPixels, 4.0)]
    [InlineData(TerrainReplayVariant.Baseline, null)]
    public void Returns_pixel_budget_only_for_sse_variants(
        TerrainReplayVariant variant, double? expected) =>
        Assert.Equal(expected, TerrainReplayOptions.SseBudgetPixels(variant));
}
