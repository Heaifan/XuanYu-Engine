namespace XuanYu.World.Terrain.Source;

using XuanYu.World;

public interface ITerrainElevationQuery
{
    ElevationQueryResult GetElevation(double latitude, double longitude,
        TerrainElevationInterpolation interpolation = TerrainElevationInterpolation.Bilinear);
}
