using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    static TerrainRenderStats Merge(TerrainRenderStats a, TerrainRenderStats b) => new(
        a.VisibleChunks + b.VisibleChunks, a.CulledChunks + b.CulledChunks,
        a.LOD0Chunks + b.LOD0Chunks, a.LOD1Chunks + b.LOD1Chunks,
        a.LOD2Chunks + b.LOD2Chunks, a.LOD3Chunks + b.LOD3Chunks,
        a.LOD4Chunks + b.LOD4Chunks, a.TerrainDrawCalls + b.TerrainDrawCalls,
        a.TerrainTriangles + b.TerrainTriangles, a.TerrainIndices + b.TerrainIndices,
        b.TerrainResidentGpuBytes, b.MeshBuildCount, b.VertexUploadCount,
        b.IndexUploadCount, b.BufferCreateCount);
}
