using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainTileSetTests
{
    [Fact]
    public void Adjacent_tiles_are_queryable_without_dense_merge()
    {
        var set = TerrainTileSet.Create([Tile("n23e121", 23, 121, 10), Tile("n23e122", 23, 122, 20)]);

        Assert.Equal(10, set.GetElevation(23.5, 121.5, TerrainElevationInterpolation.Nearest).ElevationMeters);
        Assert.Equal(20, set.GetElevation(23.5, 122.5, TerrainElevationInterpolation.Nearest).ElevationMeters);
        Assert.Equal(2, set.Tiles.Count);
    }

    [Fact]
    public void Aggregate_facts_cover_all_tiles()
    {
        var set = TerrainTileSet.Create([Tile("n22e120", 22, 120, -4), Tile("n23e121", 23, 121, 18)]);

        Assert.Equal(new TerrainGeoBounds(22, 120, 24, 122), set.Bounds);
        Assert.Equal(-4, set.MinElevation);
        Assert.Equal(18, set.MaxElevation);
    }

    [Fact]
    public void Duplicate_tile_id_is_rejected_without_overwrite()
    {
        var tiles = new[] { Tile("n23e121", 23, 121, 1), Tile("n23e121", 23, 121, 2) };

        var error = Assert.Throws<ArgumentException>(() => TerrainTileSet.Create(tiles));

        Assert.Contains("重复 TileId", error.Message);
    }

    [Fact]
    public void Overlapping_bounds_use_stable_tile_id_order()
    {
        var set = TerrainTileSet.Create([Tile("z", 23, 121, 9), Tile("a", 23, 121, 7)]);

        var sample = set.GetElevation(23.5, 121.5, TerrainElevationInterpolation.Nearest);

        Assert.Equal(7, sample.ElevationMeters);
    }

    static TerrainElevationTile Tile(string id, double south, double west, short value) =>
        new(id, new(south, west, south + 1, west + 1),
            new TerrainSourceRaster(2, 2, [value, value, value, value], [false, false, false, false]),
            new(1, 1), TerrainElevationUnit.Meter, TerrainVerticalDatum.Egm96Geoid,
            TerrainSourceFormat.NasademHgt);
}
