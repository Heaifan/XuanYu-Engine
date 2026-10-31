namespace XuanYu.Editor.UI;

internal static class TerrainDiagnosticSamples
{
    public static int Resolve(bool terrainReplayEnabled, string? requested)
    {
        var selected = terrainReplayEnabled ? requested :
            Environment.GetEnvironmentVariable("XYE_TERRAIN_RENDER_SAMPLES");
        if (string.IsNullOrWhiteSpace(selected)) return 513;
        if (int.TryParse(selected, out var samples) && samples is 513 or 1025 or 2049)
            return samples;
        throw new InvalidOperationException(
            "Terrain sample override must be 513, 1025, or 2049.");
    }
}
