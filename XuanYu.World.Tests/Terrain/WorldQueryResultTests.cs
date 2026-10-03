using XuanYu.World;

namespace XuanYu.World.Tests.Terrain;

public sealed class WorldQueryResultTests
{
    [Fact]
    public void Elevation_result_preserves_all_query_statuses()
    {
        var results = new[]
        {
            ElevationQueryResult.Valid(0),
            ElevationQueryResult.NoData,
            ElevationQueryResult.OutOfBounds,
            ElevationQueryResult.NoTerrain,
            ElevationQueryResult.InvalidBinding
        };

        Assert.Equal(WorldQueryStatus.Valid, results[0].Status);
        Assert.Equal(0, results[0].ElevationMeters);
        Assert.Equal(WorldQueryStatus.NoData, results[1].Status);
        Assert.Equal(WorldQueryStatus.OutOfBounds, results[2].Status);
        Assert.Equal(WorldQueryStatus.NoTerrain, results[3].Status);
        Assert.Equal(WorldQueryStatus.InvalidBinding, results[4].Status);
    }

    [Fact]
    public void Surface_result_rejects_value_access_for_invalid_status()
    {
        Assert.Throws<InvalidOperationException>(() =>
            SurfaceQueryResult.OutOfBounds.SurfaceZ);
    }
}
