using XuanYu.World;

namespace XuanYu.World.Terrain.Source;

public sealed class TerrainTileQuery(TerrainTileLookup lookup)
{
    public ElevationQueryResult Query(double latitude, double longitude,
        TerrainElevationInterpolation interpolation = TerrainElevationInterpolation.Bilinear)
    {
        var tile = lookup.Find(latitude, longitude);
        return tile?.GetElevation(latitude, longitude, interpolation)
            ?? ElevationQueryResult.OutOfBounds;
    }
}
