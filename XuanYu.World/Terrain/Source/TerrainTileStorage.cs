namespace XuanYu.World.Terrain.Source;

public sealed class TerrainTileStorage
{
    public TerrainTileStorage(IReadOnlyList<TerrainElevationTile> tiles)
    {
        Tiles = tiles;
        Bounds = new(tiles.Min(tile => tile.Bounds.South),
            tiles.Min(tile => tile.Bounds.West), tiles.Max(tile => tile.Bounds.North),
            tiles.Max(tile => tile.Bounds.East));
        var values = tiles.SelectMany(ValidElevations).ToArray();
        if (values.Length == 0)
            throw new ArgumentException("Tile 集合不包含有效高程。", nameof(tiles));
        MinElevation = values.Min();
        MaxElevation = values.Max();
    }

    public IReadOnlyList<TerrainElevationTile> Tiles { get; }
    public TerrainGeoBounds Bounds { get; }
    public double MinElevation { get; }
    public double MaxElevation { get; }

    static IEnumerable<double> ValidElevations(TerrainElevationTile tile) =>
        tile.Raster.ElevationMeters.Where((_, index) => !tile.Raster.NoDataMask[index]);
}
