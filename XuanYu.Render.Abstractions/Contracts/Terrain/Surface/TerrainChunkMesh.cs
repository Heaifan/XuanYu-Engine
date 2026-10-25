namespace XuanYu.Render.Abstractions;

public sealed record TerrainChunkMesh(
    IReadOnlyList<TerrainMeshVertex> Vertices,
    IReadOnlyList<uint> Indices,
    int SurfaceVertexCount,
    int SampleStride)
{
    public IReadOnlyList<TerrainMeshVertex> SurfaceVertices =>
        Vertices.Take(SurfaceVertexCount).ToArray();
}
