namespace XuanYu.World.Terrain.Source;

public sealed class TerrainTileLookup(TerrainTileStorage storage)
{
    public TerrainElevationTile? Find(double latitude, double longitude) =>
        storage.Tiles.FirstOrDefault(tile => Contains(tile.Bounds, latitude, longitude));

    static bool Contains(TerrainGeoBounds bounds, double latitude, double longitude) =>
        latitude >= bounds.South && latitude <= bounds.North &&
        longitude >= bounds.West && longitude <= bounds.East;
}
