using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapEditSessionObjectCommandTests
{
    [Fact]
    public void Region_vertex_delete_keeps_polygon_legal()
    {
        var session = new MapEditSession();
        var region = new MapRegion(MapRegionId.New(), session.ActiveRegionLayerId, "区域", MapRegionKind.Generic,
            [new(0, 0), new(100, 0), new(100, 100), new(0, 100)]);
        Assert.True(session.CreateRegion(region).IsSuccess);
        Assert.True(session.DeleteRegionVertex(region.RegionId, 1).IsSuccess);
        Assert.Equal(3, session.CurrentMap.Regions[0].Vertices.Length);
        Assert.True(session.DeleteRegionVertex(region.RegionId, 0).IsFailure);
    }

    [Fact]
    public void Road_and_marker_delete_are_undoable()
    {
        var session = new MapEditSession();
        var road = new MapRoad(MapRoadId.New(), session.ActiveRegionLayerId, "道路", "generic", [new(0, 0), new(1, 1)]);
        var marker = new MapMarker(MapMarkerId.New(), session.ActiveRegionLayerId, "标记", new(2, 2));
        Assert.True(session.CreateRoad(road).IsSuccess);
        Assert.True(session.CreateMarker(marker).IsSuccess);
        Assert.True(session.DeleteRoad(road.RoadId).IsSuccess);
        Assert.True(session.DeleteMarker(marker.MarkerId).IsSuccess);
        Assert.True(session.Undo().IsSuccess);
        Assert.Single(session.CurrentMap.Markers);
        Assert.True(session.Undo().IsSuccess);
        Assert.Single(session.CurrentMap.Roads);
    }
}
