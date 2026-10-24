using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public readonly record struct TerrainLodSelection(int Lod, double ProjectedPixels, double BaseCellPixels);

public readonly record struct TerrainVisibilityStats(
    int VisibleChunkCount, int CulledChunkCount, int Lod0Count, int Lod1Count,
    int Lod2Count, int Lod3Count, int Lod4Count);

public readonly record struct TerrainChunkVisibilityResult(
    IReadOnlyList<TerrainChunkVisibilityEntry> Entries, TerrainVisibilityStats Stats);

public readonly record struct TerrainChunkVisibilityEntry(
    int ChunkId, bool Culled, TerrainLodSelection Selection);

public static class TerrainChunkVisibility
{
    public static TerrainChunkVisibilityResult Select(
        IReadOnlyList<TerrainChunkBounds> chunks, ViewProjectionState state)
    {
        var entries = new List<TerrainChunkVisibilityEntry>(chunks.Count);
        var lods = new int[5];
        var culled = 0;
        foreach (var chunk in chunks)
        {
            if (!TerrainFrustumCuller.Intersects(state, chunk.Bounds))
            {
                entries.Add(new(chunk.ChunkId, true, new(-1, 0, 0)));
                culled++;
                continue;
            }

            var selection = TerrainLodSelector.Select(state, chunk.Bounds, -1);
            entries.Add(new(chunk.ChunkId, false, selection));
            lods[selection.Lod]++;
        }

        var stats = new TerrainVisibilityStats(entries.Count - culled, culled,
            lods[0], lods[1], lods[2], lods[3], lods[4]);
        return new TerrainChunkVisibilityResult(entries, stats);
    }
}
