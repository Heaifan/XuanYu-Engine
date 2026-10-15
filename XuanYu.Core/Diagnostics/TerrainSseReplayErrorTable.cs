using System.Text.Json;

namespace XuanYu.Core.Diagnostics;

public sealed record TerrainSseReplayError(int ChunkX, int ChunkY, int Lod,
    double? MaxVisualErrorMeters);

public sealed record TerrainSseReplayErrorDocument(string Schema, string TerrainId,
    int Revision, int Samples, IReadOnlyList<TerrainSseReplayError> Errors);

public static class TerrainSseReplayErrorTable
{
    static readonly Lazy<TerrainSseReplayErrorDocument?> Document = new(Load);
    public static string LoadStatus { get; private set; } = "NOT_LOADED";

    public static bool TryGet(string terrainId, int revision, int chunkX, int chunkY,
        int lod, out double maxVisualErrorMeters)
    {
        maxVisualErrorMeters = 0;
        var document = Document.Value;
        if (document is null || document.Schema != "XYC-B3-SSE-ERRORS/1" ||
            document.TerrainId != terrainId || document.Revision != revision) return false;
        var entry = document.Errors.FirstOrDefault(x => x.ChunkX == chunkX &&
            x.ChunkY == chunkY && x.Lod == lod);
        if (entry?.MaxVisualErrorMeters is not double error || !double.IsFinite(error) || error < 0)
            return false;
        maxVisualErrorMeters = error;
        return true;
    }

    static TerrainSseReplayErrorDocument? Load()
    {
        var path = Environment.GetEnvironmentVariable("XYE_TERRAIN_SSE_ERROR_TABLE");
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        { LoadStatus = "UNAVAILABLE"; return null; }
        try
        {
            var json = File.ReadAllText(path);
            var document = JsonSerializer.Deserialize<TerrainSseReplayErrorDocument>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            LoadStatus = document?.Schema == "XYC-B3-SSE-ERRORS/1" ? "READY" : "INVALID_SCHEMA";
            return document;
        }
        catch (Exception error) when (error is IOException or JsonException or UnauthorizedAccessException)
        { LoadStatus = $"INVALID:{error.GetType().Name}"; return null; }
    }
}
