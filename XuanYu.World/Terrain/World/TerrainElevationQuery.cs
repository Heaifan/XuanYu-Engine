using XuanYu.World;

namespace XuanYu.World.Terrain;

public sealed class TerrainElevationQuery(TerrainWorldStorage storage)
{
    public ElevationQueryResult Query(TerrainSampleCoordinate coordinate)
    {
        var sample = storage.Read(coordinate);
        return sample.IsValid
            ? ElevationQueryResult.Valid(sample.Meters)
            : ElevationQueryResult.NoData;
    }
}
