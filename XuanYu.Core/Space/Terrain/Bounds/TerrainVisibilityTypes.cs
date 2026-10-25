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

