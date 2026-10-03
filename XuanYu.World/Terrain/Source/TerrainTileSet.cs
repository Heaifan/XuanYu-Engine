namespace XuanYu.World.Terrain.Source;

using XuanYu.World;

public sealed class TerrainTileSet : ITerrainElevationQuery
{
    TerrainTileSet(IReadOnlyList<TerrainElevationTile> tiles)
    {
        Storage = new TerrainTileStorage(tiles);
        Lookup = new TerrainTileLookup(Storage);
        Query = new TerrainTileQuery(Lookup);
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

    public TerrainTileStorage Storage { get; }
    public TerrainTileLookup Lookup { get; }
    public TerrainTileQuery Query { get; }
    public IReadOnlyList<TerrainElevationTile> Tiles => Storage.Tiles;
    public TerrainGeoBounds Bounds => Storage.Bounds;
    public double MinElevation => Storage.MinElevation;
    public double MaxElevation => Storage.MaxElevation;

    public ElevationQueryResult GetElevation(double latitude, double longitude,
        TerrainElevationInterpolation interpolation = TerrainElevationInterpolation.Bilinear)
        => Query.Query(latitude, longitude, interpolation);
}
