namespace XuanYu.World.Terrain.Source;

public interface ITerrainElevationQuery
{
    TerrainElevationResult GetElevation(double latitude, double longitude,
        TerrainElevationInterpolation interpolation = TerrainElevationInterpolation.Bilinear);
}
