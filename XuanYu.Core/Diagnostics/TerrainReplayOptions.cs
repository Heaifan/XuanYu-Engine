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
    LodBiasOne
}

public static class TerrainReplayOptions
{
    static readonly bool EnabledAtStartup = ViewportProbe.Enabled &&
        Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY") == "1";

    public static bool Enabled => EnabledAtStartup;

    public static double? FarPlaneOverride
    {
        get => Enabled && TryParseFarPlane(
            Environment.GetEnvironmentVariable("XYE_TERRAIN_FAR_PLANE"), out var far)
            ? far : null;
    }

    public static TerrainReplayVariant Variant => Enabled &&
        TryParseVariant(Environment.GetEnvironmentVariable("XYE_TERRAIN_AB"), out var variant)
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

    public static CameraState ApplyCamera(CameraState camera, TerrainReplayVariant variant) =>
        ApplyCamera(camera, variant, FarPlaneOverride);

    public static CameraState ApplyCamera(CameraState camera, TerrainReplayVariant variant,
        double? farPlaneOverride)
    {
        var near = variant == TerrainReplayVariant.ReducedNearPlane
            ? global::System.Math.Max(0.01, camera.NearPlane * 0.5) : camera.NearPlane;
        var far = variant == TerrainReplayVariant.ExpandedFarPlane ? camera.FarPlane * 2.0
            : farPlaneOverride is > 0 && farPlaneOverride > camera.NearPlane
                ? farPlaneOverride.Value : camera.FarPlane;
        if (near == camera.NearPlane && far == camera.FarPlane) return camera;
        return new(camera.Position, camera.Forward, camera.Up, camera.VerticalFovDegrees,
            near, far, camera.Revision, camera.Mode, camera.OrthographicScale);
    }

    public static bool ShouldCull(bool intersects, TerrainReplayVariant variant) =>
        !intersects && variant != TerrainReplayVariant.DisableChunkCulling;

    public static int SelectLod(int selectedLod, TerrainReplayVariant variant) => variant switch
    {
        TerrainReplayVariant.FixedLod0 => 0,
        TerrainReplayVariant.LodBiasOne => global::System.Math.Min(4, selectedLod + 1),
        _ => selectedLod
    };
}
