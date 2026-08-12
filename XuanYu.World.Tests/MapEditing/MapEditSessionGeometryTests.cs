using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapEditSessionGeometryTests
{
    [Fact]
    public void Region_vertex_edit_is_one_history_entry_and_round_trips()
    {
        var session = new MapEditSession();
        var region = new MapRegion(MapRegionId.New(), session.ActiveRegionLayerId, "区域",
            MapRegionKind.Generic, [new(0, 0), new(100, 0), new(100, 100)]);
        Assert.True(session.CreateRegion(region).IsSuccess);
        var moved = region.Vertices.SetItem(1, new MapPoint(120, 0));

        Assert.True(session.EditRegionVertices(region.RegionId, moved).IsSuccess);
        Assert.Equal(2, session.CurrentStateId);
        Assert.Equal(moved, session.CurrentMap.Regions[0].Vertices);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal(region.Vertices, session.CurrentMap.Regions[0].Vertices);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(moved, session.CurrentMap.Regions[0].Vertices);
    }

    [Fact]
    public void Invalid_region_edit_is_rejected_without_history()
    {
        var session = new MapEditSession();
        var region = new MapRegion(MapRegionId.New(), session.ActiveRegionLayerId, "区域",
            MapRegionKind.Generic, [new(0, 0), new(100, 0), new(100, 100), new(0, 100)]);
        Assert.True(session.CreateRegion(region).IsSuccess);
        var before = session.CurrentMap;

        var result = session.EditRegionVertices(region.RegionId,
            [new(0, 0), new(100, 100), new(100, 0), new(0, 100)]);

        Assert.True(result.IsFailure);
        Assert.Equal("InvalidMap", result.Error!.Value.Code);
        Assert.Same(before, session.CurrentMap);
        Assert.Equal(1, session.CurrentStateId);
    }

    [Fact]
    public void Road_vertex_edit_rejects_adjacent_duplicate()
    {
        var session = new MapEditSession();
        var road = new MapRoad(MapRoadId.New(), session.ActiveRegionLayerId, "道路", "generic",
            ImmutableArray.Create(new MapPoint(0, 0), new MapPoint(100, 0)));
        Assert.True(session.CreateRoad(road).IsSuccess);

        var result = session.EditRoadPoints(road.RoadId, [new(0, 0), new(0, 0)]);

        Assert.True(result.IsFailure);
        Assert.Equal("InvalidMap", result.Error!.Value.Code);
        Assert.Equal(1, session.CurrentStateId);
    }
}
