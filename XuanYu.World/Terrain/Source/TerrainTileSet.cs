namespace XuanYu.World.Terrain.Source;

public sealed class TerrainTileSet : ITerrainElevationQuery
{
    readonly IReadOnlyList<TerrainElevationTile> _tiles;

    TerrainTileSet(IReadOnlyList<TerrainElevationTile> tiles)
    {
        _tiles = tiles;
        Bounds = AggregateBounds(tiles);
        var range = AggregateElevationRange(tiles);
        MinElevation = range.Min; MaxElevation = range.Max;
    }

    public static TerrainTileSet Create(IEnumerable<TerrainElevationTile> tiles)
    {
        ArgumentNullException.ThrowIfNull(tiles);
        var ordered = tiles.OrderBy(tile => tile.TileId, StringComparer.Ordinal).ToArray();
        var duplicate = ordered.GroupBy(tile => tile.TileId, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null) throw new ArgumentException($"重复 TileId：{duplicate.Key}。", nameof(tiles));
        return new(ordered);
    }

    public IReadOnlyList<TerrainElevationTile> Tiles => _tiles;
    public TerrainGeoBounds Bounds { get; }
    public double MinElevation { get; }
    public double MaxElevation { get; }

    public TerrainElevationResult GetElevation(double latitude, double longitude,
        TerrainElevationInterpolation interpolation = TerrainElevationInterpolation.Bilinear)
    {
        var tile = _tiles.FirstOrDefault(item => Contains(item.Bounds, latitude, longitude));
        return tile?.GetElevation(latitude, longitude, interpolation) ?? TerrainElevationResult.Invalid;
    }

    static bool Contains(TerrainGeoBounds bounds, double latitude, double longitude) =>
        latitude >= bounds.South && latitude <= bounds.North &&
        longitude >= bounds.West && longitude <= bounds.East;

    static (double Min, double Max) AggregateElevationRange(
        IReadOnlyList<TerrainElevationTile> tiles)
    {
        var min = double.PositiveInfinity; var max = double.NegativeInfinity;
        foreach (var tile in tiles)
            for (var index = 0; index < tile.Raster.ElevationMeters.Count; index++)
            {
                if (tile.Raster.NoDataMask[index]) continue;
                var value = tile.Raster.ElevationMeters[index];
                min = Math.Min(min, value); max = Math.Max(max, value);
            }
        if (!double.IsFinite(min)) throw new ArgumentException("Tile 集合不包含有效高程。", nameof(tiles));
        return (min, max);
    }

    static TerrainGeoBounds AggregateBounds(IReadOnlyList<TerrainElevationTile> tiles) => new(
        tiles.Min(tile => tile.Bounds.South), tiles.Min(tile => tile.Bounds.West),
        tiles.Max(tile => tile.Bounds.North), tiles.Max(tile => tile.Bounds.East));
}
