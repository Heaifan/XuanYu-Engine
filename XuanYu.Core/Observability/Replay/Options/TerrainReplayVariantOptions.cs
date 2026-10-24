namespace XuanYu.Core.Diagnostics;

public static partial class TerrainReplayOptions
{
    public static bool ShouldCull(bool intersects, TerrainReplayVariant variant) =>
        !intersects && variant != TerrainReplayVariant.DisableChunkCulling;

    public static double? SseBudgetPixels(TerrainReplayVariant variant) => variant switch
    {
        TerrainReplayVariant.SseBudgetHalfPixel => 0.5,
        TerrainReplayVariant.SseBudgetOnePixel => 1.0,
        TerrainReplayVariant.SseBudgetTwoPixels => 2.0,
        TerrainReplayVariant.SseBudgetFourPixels => 4.0,
        _ => null
    };

    public static int SelectLod(int selectedLod, TerrainReplayVariant variant) => variant switch
    {
        TerrainReplayVariant.FixedLod0 => 0,
        TerrainReplayVariant.LodBiasOne => global::System.Math.Min(4, selectedLod + 1),
        _ => selectedLod
    };
}
