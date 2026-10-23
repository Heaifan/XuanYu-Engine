using System.Globalization;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Diagnostics;

public enum TerrainReplayVariant
{
    Baseline,
    ExpandedFarPlane,
    DisableChunkCulling,
    FixedLod0,
    ReducedNearPlane,
    LodBiasOne,
    SseBudgetHalfPixel,
    SseBudgetOnePixel,
    SseBudgetTwoPixels,
    SseBudgetFourPixels
}

public static partial class TerrainReplayOptions
{
    static readonly bool EnabledAtStartup = ViewportProbe.Enabled &&
        Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY") == "1";

    public static bool Enabled => EnabledAtStartup;

    public static double? FarPlaneOverride => Enabled && TryParseFarPlane(
        Environment.GetEnvironmentVariable("XYE_TERRAIN_FAR_PLANE"), out var far)
        ? far : null;

    public static TerrainReplayVariant Variant => Enabled && TryParseVariant(
        Environment.GetEnvironmentVariable("XYE_TERRAIN_AB"), out var variant)
        ? variant : TerrainReplayVariant.Baseline;

    public static int ChunkCellsOverride => Enabled && int.TryParse(
        Environment.GetEnvironmentVariable("XYE_TERRAIN_CHUNK_CELLS"), out var cells) && cells > 0
        ? cells : 240;

    public static bool TryParseVariant(string? value, out TerrainReplayVariant variant)
    {
        variant = value switch
        {
            "baseline" => TerrainReplayVariant.Baseline,
            "far-expanded" => TerrainReplayVariant.ExpandedFarPlane,
            "cull-off" => TerrainReplayVariant.DisableChunkCulling,
            "lod0" => TerrainReplayVariant.FixedLod0,
            "lod-bias-1" => TerrainReplayVariant.LodBiasOne,
            "sse-0.5" => TerrainReplayVariant.SseBudgetHalfPixel,
            "sse-1" => TerrainReplayVariant.SseBudgetOnePixel,
            "sse-2" => TerrainReplayVariant.SseBudgetTwoPixels,
            "sse-4" => TerrainReplayVariant.SseBudgetFourPixels,
            "near-small" => TerrainReplayVariant.ReducedNearPlane,
            _ => (TerrainReplayVariant)(-1)
        };
        return (int)variant >= 0;
    }

    public static bool TryParseFarPlane(string? value, out double farPlane)
    {
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture,
            out farPlane) && double.IsFinite(farPlane) && farPlane > 0)
            return true;
        farPlane = 0;
        return false;
    }
}
