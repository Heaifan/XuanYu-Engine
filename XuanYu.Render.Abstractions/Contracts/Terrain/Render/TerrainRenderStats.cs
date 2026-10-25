namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainRenderStats(
    int VisibleChunks, int CulledChunks, int LOD0Chunks, int LOD1Chunks,
    int LOD2Chunks, int LOD3Chunks, int LOD4Chunks,
    int TerrainDrawCalls, int TerrainTriangles, int TerrainIndices,
    long TerrainResidentGpuBytes, int MeshBuildCount, int VertexUploadCount,
    int IndexUploadCount, int BufferCreateCount)
{
    public static TerrainRenderStats From(IReadOnlyList<TerrainChunkDraw> draws, int culled = 0) =>
        new(draws.Count, culled, Count(draws, 0), Count(draws, 1), Count(draws, 2),
            Count(draws, 3), Count(draws, 4), draws.Count,
            draws.Sum(x => x.TriangleCount), draws.Sum(x => x.IndexCount), 0, 0, 0, 0, 0);

    static int Count(IReadOnlyList<TerrainChunkDraw> draws, int lod) => draws.Count(x => x.Lod == lod);
}
