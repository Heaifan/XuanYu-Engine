namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainChunkGpuKey(
    string TerrainId, int Revision, double VerticalExaggeration,
    int ChunkX, int ChunkY, TerrainLodLevel Lod)
{
    public TerrainChunkGpuKey(string terrainId, int revision, double verticalExaggeration,
        int chunkX, int chunkY, int lod) : this(terrainId, revision, verticalExaggeration,
        chunkX, chunkY, (TerrainLodLevel)lod) { }
}
