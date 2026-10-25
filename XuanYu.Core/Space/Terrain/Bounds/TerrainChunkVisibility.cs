using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

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
            if (selection.Lod is < TerrainLodSelector.MinLod or > TerrainLodSelector.MaxLod)
                throw new InvalidOperationException("地形 LOD 越界。");
            lods[selection.Lod]++;
        }

        var stats = new TerrainVisibilityStats(entries.Count - culled, culled,
            lods[0], lods[1], lods[2], lods[3], lods[4]);
        return new TerrainChunkVisibilityResult(entries, stats);
    }
}
