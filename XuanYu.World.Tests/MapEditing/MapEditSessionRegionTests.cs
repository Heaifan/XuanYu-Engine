using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapEditSessionRegionTests
{
    [Fact]
    public void Create_region_is_one_history_entry_and_redo_keeps_id()
    {
        var session = new MapEditSession();
        var region = NewRegion(session);

        Assert.True(session.CreateRegion(region).IsSuccess);
        Assert.Equal(1, session.CurrentStateId);
        Assert.Single(session.CurrentMap.Regions);
        Assert.Equal(region.RegionId, session.CurrentMap.Regions[0].RegionId);
        Assert.True(session.Undo().IsSuccess);
        Assert.Empty(session.CurrentMap.Regions);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(region.RegionId, session.CurrentMap.Regions[0].RegionId);
    }

    [Fact]
    public void Delete_region_undo_restores_same_snapshot()
    {
        var session = new MapEditSession();
        var region = NewRegion(session);
        session.CreateRegion(region);
        var beforeDelete = session.CurrentMap.Regions[0];

        Assert.True(session.DeleteRegion(region.RegionId).IsSuccess);
        Assert.Equal(2, session.CurrentStateId);
        Assert.Empty(session.CurrentMap.Regions);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal(beforeDelete, session.CurrentMap.Regions[0]);
        Assert.True(session.Redo().IsSuccess);
        Assert.Empty(session.CurrentMap.Regions);
    }

    static MapRegion NewRegion(MapEditSession session) => new(
        MapRegionId.New(), session.CurrentMap.Layers[2].LayerId, "部署区A", MapRegionKind.Deployment,
        ImmutableArray.Create(new MapPoint(-100, -100), new MapPoint(100, -100),
            new MapPoint(100, 100), new MapPoint(-100, 100)));
}
