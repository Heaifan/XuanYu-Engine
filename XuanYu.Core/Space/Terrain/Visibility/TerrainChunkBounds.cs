using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public readonly record struct TerrainChunkBounds
{
    public TerrainChunkBounds(int chunkId, SpatialAabb bounds, int cellCount = 240)
    {
        if (cellCount != 240) throw new ArgumentOutOfRangeException(nameof(cellCount));
        ChunkId = chunkId;
        Bounds = bounds;
        CellCount = cellCount;
    }

    public int ChunkId { get; }
    public SpatialAabb Bounds { get; }
    public int CellCount { get; }
}
