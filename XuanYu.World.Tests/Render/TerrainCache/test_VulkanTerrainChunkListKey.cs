using XuanYu.Render.Vulkan.Render.Terrain;

namespace XuanYu.World.Tests.Render.TerrainCache;

public sealed class VulkanTerrainChunkListKeyTests
{
    [Fact]
    public void Chunk_cell_override_is_part_of_cache_identity()
    {
        var baseline = new TerrainChunkListKey("terrain", 4, 240);
        var replay = new TerrainChunkListKey("terrain", 4, 120);

        Assert.NotEqual(baseline, replay);
    }
}
