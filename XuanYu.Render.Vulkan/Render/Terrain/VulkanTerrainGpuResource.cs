using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Terrain;

sealed class VulkanTerrainGpuResource : IDisposable
{
    readonly VulkanStaticModelBuffer _vertices;
    readonly VulkanStaticModelBuffer _indices;
    VulkanTerrainGpuResource(VulkanStaticModelBuffer vertices, VulkanStaticModelBuffer indices, uint count) =>
        (_vertices, _indices, IndexCount) = (vertices, indices, count);
    public VulkanStaticModelBuffer Vertices => _vertices;
    public VulkanStaticModelBuffer Indices => _indices;
    public uint IndexCount { get; }
    public ulong GpuBytes => _vertices.CapacityBytes + _indices.CapacityBytes;

    public static VulkanTerrainGpuResource? Create(Vk vk, VulkanDeviceOwner device,
        TerrainRenderResource resource, TerrainChunkDescriptor chunk, TerrainLodLevel lod,
        TerrainRenderTransform transform, out string error)
    {
        if (!chunk.Matches(resource.TerrainId, resource.Revision))
        {
            error = "Terrain Chunk 的 TerrainId 或 Revision 不匹配。";
            return null;
        }
        if (!chunk.HasValidSampleRange(resource.Heightfield))
        {
            error = "Terrain Chunk 的采样范围不合法。";
            return null;
        }
        var mesh = TerrainChunkMeshBuilder.Build(resource.Heightfield, chunk, lod,
            transform.VerticalExaggeration);
        var vertices = mesh.Vertices.Select(v => new VulkanStaticModelVertex(
            (float)(v.X + chunk.StartSampleX * resource.CellSizeMeters),
            (float)(v.Y + chunk.StartSampleY * resource.CellSizeYMeters), (float)v.Z,
            (float)v.Nx, (float)v.Ny, (float)v.Nz, 0, 0)).ToArray();
        var vb = VulkanStaticModelBuffer.Create(vk, device, vertices, BufferUsageFlags.VertexBufferBit, out error);
        if (vb is null) return null;
        var ib = VulkanStaticModelBuffer.Create(vk, device, mesh.Indices.ToArray(), BufferUsageFlags.IndexBufferBit, out error);
        if (ib is null) { vb.Dispose(); return null; }
        return new VulkanTerrainGpuResource(vb, ib, (uint)mesh.Indices.Count);
    }

    public void Dispose() { _vertices.Dispose(); _indices.Dispose(); }
}
