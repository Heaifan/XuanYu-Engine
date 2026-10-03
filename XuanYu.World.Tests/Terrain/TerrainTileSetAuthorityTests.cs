using XuanYu.World;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainTileSetAuthorityTests
{
    [Fact]
    public void Tile_set_exposes_separate_storage_lookup_and_query_authorities()
    {
        var set = TerrainTileSet.Create([Tile("a", 23, 121, 10)]);

        Assert.Single(set.Storage.Tiles);
        Assert.NotNull(set.Lookup.Find(23.5, 121.5));
        Assert.Equal(WorldQueryStatus.Valid,
            set.Query.Query(23.5, 121.5, TerrainElevationInterpolation.Nearest).Status);
    }

    [Fact]
    public void Tile_query_distinguishes_out_of_bounds_and_nodata()
    {
        var tile = new TerrainElevationTile("a", new(23, 121, 24, 122),
            new TerrainSourceRaster(2, 2, [short.MinValue, 2, 3, 4],
                [true, false, false, false]), new(1, 1),
            TerrainElevationUnit.Meter, TerrainVerticalDatum.Egm96Geoid,
            TerrainSourceFormat.NasademHgt);

        Assert.Equal(WorldQueryStatus.OutOfBounds,
            tile.GetElevation(22.9, 121.5).Status);
        Assert.Equal(WorldQueryStatus.NoData,
            tile.GetElevation(24, 121).Status);
    }

    static TerrainElevationTile Tile(string id, double south, double west, short value) =>
        new(id, new(south, west, south + 1, west + 1),
            new TerrainSourceRaster(2, 2, [value, value, value, value],
                [false, false, false, false]), new(1, 1),
            TerrainElevationUnit.Meter, TerrainVerticalDatum.Egm96Geoid,
            TerrainSourceFormat.NasademHgt);
}
