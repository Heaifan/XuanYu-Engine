using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainHgtWorldTests
{
    [Fact]
    public void Imported_tile_registers_in_world_and_query_returns_fixture_value()
    {
        var reader = (IHgtReader)new HgtTerrainElevationTileReader();
        using var stream = TerrainImportFixture.HgtStream(10, 20, 30, 40);
        var tile = reader.Read(stream, "n23e121");
        var world = TerrainWorld.FromElevationTile(tile);
        Assert.Equal(40, tile.GetElevation(23, 122).ElevationMeters);
        Assert.Equal(10, world.QueryHeight(new(0, 0)));
        Assert.True(world.TryGetFinalHeight(new(1, 0), out var height));
        Assert.Equal(20, height);
    }

    [Fact]
    public void Projection_and_render_resource_preserve_different_elevations()
    {
        var reader = (IHgtReader)new HgtTerrainElevationTileReader();
        using var stream = TerrainImportFixture.HgtStream(10, 20, 30, 40);
        var world = TerrainWorld.FromElevationTile(reader.Read(stream, "n23e121"));

        TerrainHeightfield projection = world.ToHeightfield();
        TerrainRenderResource resource = world.ToRenderSnapshot("n23e121", 1);

        Assert.Equal(2, projection.Width);
        Assert.NotEqual(projection.ElevationAt(0, 0), projection.ElevationAt(1, 1));
        Assert.Equal("n23e121", resource.TerrainId);
        Assert.Equal(6, resource.TriangleIndexCount);
        Assert.NotEqual(resource.Heightfield.ElevationAt(0, 0),
            resource.Heightfield.ElevationAt(1, 1));
    }

    [Fact]
    public void Projection_preserves_world_resolution_in_render_resource()
    {
        var world = new TerrainWorld(
            TerrainHeightLayer.CreateBase(2, 2, [1, 2, 3, 4], new bool[4]),
            new TerrainMetadata(2, 2, new(2, 3), 1, 4, null, 0,
                new(0, 0, 2, 3)), null);

        var resource = world.ToRenderSnapshot("resolution", 4);

        Assert.Equal(2, resource.CellSizeMeters);
        Assert.Equal(2, resource.Metadata.ResolutionX);
        Assert.Equal(3, resource.Metadata.ResolutionY);
    }

    [Fact]
    public void Render_mesh_preserves_distinct_x_and_y_sample_spacing()
    {
        var world = new TerrainWorld(
            TerrainHeightLayer.CreateBase(3, 4, Enumerable.Range(0, 12)
                .Select(value => (double)value).ToArray(), new bool[12]),
            new TerrainMetadata(3, 4, new(2, 3), 0, 11, null, 0,
                new(0, 0, 2, 3)), null);
        var resource = world.ToRenderSnapshot("anisotropic", 1);
        var chunk = TerrainChunkPartitioner.Partition(
            resource.Heightfield, resource.TerrainId, resource.Revision).Single();
        var mesh = TerrainChunkMeshBuilder.Build(
            resource.Heightfield, chunk, TerrainLodLevel.Lod0, 1);

        Assert.Equal(4, mesh.SurfaceVertices[^1].X);
        Assert.Equal(9, mesh.SurfaceVertices[^1].Y);
    }
}
