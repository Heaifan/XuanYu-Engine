using XuanYu.Core.Map;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

public sealed class TerrainMultiTileRenderContractTests
{
    [Fact]
    public void Projection_accepts_zero_one_and_many_terrain_resources()
    {
        Assert.Empty(Projection([]).TerrainResources);
        Assert.Single(Projection([Resource("a")]).TerrainResources);
        Assert.Equal(["a", "b", "c"], Projection([Resource("a"), Resource("b"), Resource("c")])
            .TerrainResources.Select(resource => resource.TerrainId));
    }

    [Fact]
    public void Draw_plan_contains_one_entry_per_terrain_in_stable_order()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection([Resource("a"), Resource("b"), Resource("c")]));
        Assert.Equal(3, plan.Count(entry => entry.Kind == RenderDrawKind.Terrain));
        Assert.Equal([0, 1, 2], plan.Where(entry => entry.Kind == RenderDrawKind.Terrain)
            .Select(entry => entry.EntityIndex));
    }

    [Fact]
    public async Task Ui_projection_projects_every_imported_tile_without_dense_merge()
    {
        var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}"));
        var first = Path.Combine(directory.FullName, "n23e121.hgt");
        var second = Path.Combine(directory.FullName, "n23e122.hgt");
        try
        {
            File.WriteAllBytes(first, HgtBytes(10)); File.WriteAllBytes(second, HgtBytes(20));
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            Assert.True(await vm.ImportTerrainSourcesAsync([first, second]));
            Assert.Equal(["n23e121", "n23e122"], vm.RenderProjection.Projection.TerrainResources
                .Select(resource => resource.TerrainId));
            Assert.Equal(2, vm.RenderProjection.Projection.TerrainResources.Count);
            Assert.All(vm.RenderProjection.Projection.TerrainResources,
                resource => Assert.Equal(2, resource.Heightfield.Width));
        }
        finally { directory.Delete(true); }
    }

    [Fact]
    public void Empty_terrain_collection_is_a_valid_projection()
    {
        var projection = Projection([]);
        Assert.False(projection.HasTerrain);
        Assert.DoesNotContain(RenderDrawPlan.GetFrameDrawPlan(projection), entry => entry.Kind == RenderDrawKind.Terrain);
    }

    [Fact]
    public void World_surface_terrain_suppresses_map_ground_in_frame_plan()
    {
        var projection = Projection([Resource("terrain")]) with
        { Map = new MapRenderSnapshot("map", 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, 1) };

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        Assert.Contains(plan, entry => entry.Kind == RenderDrawKind.Terrain);
        Assert.DoesNotContain(plan, entry => entry.Kind == RenderDrawKind.MapGround);
    }

    [Fact]
    public void Gpu_cache_uses_tile_id_and_retain_only_lifecycle_contract()
    {
        var source = File.ReadAllText(Find("XuanYu.Render.Vulkan", "Render", "Terrain", "VulkanTerrainGpuCache.cs"));
        Assert.Contains("TerrainChunkGpuKey", source);
        Assert.Contains("RetainOnly", source);
        Assert.Contains("resource.TerrainId", source);
        Assert.Contains("TerrainChunkPartitioner.Partition", source);
    }

    static RenderProjection Projection(IReadOnlyList<TerrainRenderResource> terrains) =>
        new(default, [], false, default, Terrains: terrains);

    static TerrainRenderResource Resource(string id) =>
        new(id, 1, new TerrainHeightfield(2, 2, [1, 2, 3, 4]));

    static byte[] HgtBytes(short value) =>
        Enumerable.Range(0, 4).SelectMany(_ => new[] { (byte)(value >> 8), (byte)value }).ToArray();

    static string Find(params string[] parts) =>
        Path.GetFullPath(Path.Combine([AppContext.BaseDirectory, "..", "..", "..", "..", .. parts]));
}
