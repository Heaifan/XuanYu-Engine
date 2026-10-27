using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.Terrain;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanTerrainMeshIndexBoundsTests
{
    [Fact]
    public void Valid_mesh_indices_are_accepted()
    {
        var mesh = Mesh([0, 1, 2]);

        Assert.True(VulkanTerrainGpuMeshIndexValidator.TryValidate(mesh, out var error));
        Assert.Empty(error);
    }

    [Fact]
    public void Index_at_vertex_count_is_rejected_before_gpu_upload()
    {
        var mesh = Mesh([0, 1, 3]);

        Assert.False(VulkanTerrainGpuMeshIndexValidator.TryValidate(mesh, out var error));
        Assert.Contains("索引越界", error);
    }

    static TerrainChunkMesh Mesh(IReadOnlyList<uint> indices) => new(
        [Vertex(), Vertex(), Vertex()], indices, 3, 1);

    static TerrainMeshVertex Vertex() => new(0, 0, 0, 0, 0, 1);
}
