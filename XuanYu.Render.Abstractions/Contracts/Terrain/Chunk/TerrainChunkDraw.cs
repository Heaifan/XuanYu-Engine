namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainChunkDraw(
    int ChunkX, int ChunkY, int Lod, int TriangleCount, int IndexCount);
