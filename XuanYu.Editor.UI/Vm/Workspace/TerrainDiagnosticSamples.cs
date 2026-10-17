namespace XuanYu.Editor.UI;

internal static class TerrainDiagnosticSamples
{
    public static int Resolve(bool terrainReplayEnabled, string? requested)
    {
        if (!terrainReplayEnabled || string.IsNullOrWhiteSpace(requested)) return 513;
        if (int.TryParse(requested, out var samples) && samples is 513 or 1025 or 2049)
            return samples;
        throw new InvalidOperationException(
            "Terrain replay sample override must be 513, 1025, or 2049.");
    }
}
