using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.Terrain;

namespace XuanYu.World.Tests.Render;

public sealed class TerrainChunkDrawContractTests
{
    [Fact]
    public void Fixed_chunk_partition_splits_3601_samples_into_225_chunks()
    {
        var chunks = TerrainChunkPartitioner.Partition(Resource(3601, 3601).Heightfield, "terrain", 1);
        Assert.Equal(225, chunks.Count);
        Assert.Equal(240, chunks[0].CellCountX);
        Assert.Equal(241, chunks[^1].SampleCountX);
    }

    [Fact]
    public void Chunk_lod_mesh_uses_stride_without_building_whole_heightfield()
    {
        var resource = Resource(481, 481);
        var chunk = TerrainChunkPartitioner.Partition(resource.Heightfield, "terrain", resource.Revision)[0];
        var mesh = TerrainChunkMeshBuilder.Build(resource.Heightfield, chunk, TerrainLodLevel.Lod2, 1);
        Assert.Equal((chunk.CellCountX / 4 + 1) * (chunk.CellCountY / 4 + 1), mesh.SurfaceVertexCount);
    }

    [Fact]
    public void Cache_key_distinguishes_lod_and_revision()
    {
        var a = new TerrainChunkGpuKey("t", 1, 1, 0, 0, TerrainLodLevel.Lod2);
        Assert.NotEqual(a, a with { Lod = TerrainLodLevel.Lod3 });
        Assert.NotEqual(a, a with { Revision = 2 });
    }

    [Fact]
    public void Cache_retention_matches_terrain_revision()
    {
        var resource = Resource(2, 2);
        var current = new TerrainChunkGpuKey("terrain", 1, 1, 0, 0, TerrainLodLevel.Lod0);

        Assert.True(VulkanTerrainGpuCache.BelongsTo(current, resource));
        Assert.False(VulkanTerrainGpuCache.BelongsTo(current with { Revision = 2 }, resource));
    }

    [Fact]
    public void Terrain_stats_sum_selected_chunks()
    {
        var stats = TerrainRenderStats.From([new TerrainChunkDraw(0, 0, 1, 10, 20),
            new TerrainChunkDraw(1, 0, 2, 30, 60)]);
        Assert.Equal(2, stats.VisibleChunks);
        Assert.Equal(1, stats.LOD1Chunks);
        Assert.Equal(1, stats.LOD2Chunks);
        Assert.Equal(40, stats.TerrainTriangles);
        Assert.Equal(80, stats.TerrainIndices);
    }

    static TerrainRenderResource Resource(int width, int height) =>
        new("terrain", 1, new TerrainHeightfield(width, height,
            Enumerable.Repeat(0d, width * height).ToArray()));
}
