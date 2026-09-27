using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainChunkQueryPreservationTests
{
    [Fact]
    public void TerrainElevationQueryUnchanged()
    {
        var tile = TerrainElevationTile.Create(2, 2, [10, 20, 30, 40],
            new(23, 24, 121, 122), new(1, 1));
        var before = tile.GetElevation(23.5, 121.5).ElevationMeters;
        var field = new TerrainHeightfield(2, 2, tile.Raster.ElevationMeters,
            tile.Raster.NoDataMask, TerrainRenderMetadata.Empty, 30d);
        var chunk = TerrainChunkPartitioner.Partition(field, tile.TileId, 1)[0];
        _ = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 1d);
        var after = tile.GetElevation(23.5, 121.5).ElevationMeters;
        Assert.Equal(before, after);
        Assert.Equal(4, field.ElevationMeters.Count);
    }
}
