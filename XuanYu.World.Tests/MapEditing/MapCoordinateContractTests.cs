using XuanYu.Core.Math;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapCoordinateContractTests
{
    [Fact]
    public void Map_point_round_trips_through_world_coordinates()
    {
        var mapPoint = new MapPoint(-2500, 1500);

        var worldPoint = MapCoordinateContract.MapToWorld(mapPoint, 12);
        var roundTrip = MapCoordinateContract.WorldToMap(worldPoint);

        Assert.Equal(new Vector3d(-2500, 1500, 12), worldPoint);
        Assert.Equal(mapPoint, roundTrip);
    }
}
