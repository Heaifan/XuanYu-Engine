using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSpatialIndexLifecycleTests
{
    static readonly RegionSpatialBounds Near = new(-20, -20, 20, 20);
    static readonly RegionSpatialBounds Moved = new(980, -20, 1020, 20);

    [Fact]
    public void Create_edit_and_delete_update_local_candidates()
    {
        var session = new MapEditSession();
        var region = Region(session, 0);
        Assert.True(session.CreateRegion(region).IsSuccess);
        Assert.Equal([region.RegionId], session.QueryLocalRegions(Near).ToArray());

        var points = region.Vertices.Select(point => new MapPoint(point.X + 1000, point.Y)).ToImmutableArray();
        Assert.True(session.EditRegionVertices(region.RegionId, points).IsSuccess);
        Assert.Empty(session.QueryLocalRegions(Near));
        Assert.Equal([region.RegionId], session.QueryLocalRegions(Moved).ToArray());

        Assert.True(session.DeleteRegion(region.RegionId).IsSuccess);
        Assert.Empty(session.QueryLocalRegions(Moved));
    }

    [Fact]
    public void Replace_new_and_runtime_projection_rebuild_before_events()
    {
        var session = new MapEditSession();
        var oldRegion = Region(session, 1000);
        Assert.True(session.CreateRegion(oldRegion).IsSuccess);
        var replacement = Region(session, 0);
        ImmutableArray<MapRegionId> observed = [];
        session.ContentChanged += _ => observed = session.QueryLocalRegions(Near);

        Assert.True(session.ReplaceCurrentMap(
            session.CurrentMap with { Regions = [replacement] }, false, null).IsSuccess);
        Assert.Equal([replacement.RegionId], observed.ToArray());
        Assert.Empty(session.QueryLocalRegions(Moved));
        Assert.Equal([replacement.RegionId], session.QueryLocalRegions(Near).ToArray());

        Assert.True(session.CreateNewMap().IsSuccess);
        Assert.Empty(observed);
        Assert.Equal(0, session.IndexedRegionCount);
        var projected = Region(session, 0);
        Assert.True(session.CreateRegion(projected).IsSuccess);
        Assert.Equal([projected.RegionId], observed.ToArray());
        Assert.True(session.ApplyRuntimeLayerProjection(session.CurrentMap with { Regions = [] }).IsSuccess);
        Assert.Empty(observed);
    }

    [Fact]
    public void Undo_and_redo_publish_matching_index_state()
    {
        var session = new MapEditSession();
        var region = Region(session, 0);
        ImmutableArray<MapRegionId> observed = [];
        session.ContentChanged += _ => observed = session.QueryLocalRegions(Near);
        Assert.True(session.CreateRegion(region).IsSuccess);
        Assert.Equal([region.RegionId], observed.ToArray());
        Assert.True(session.DeleteRegion(region.RegionId).IsSuccess);
        Assert.Empty(observed);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal([region.RegionId], observed.ToArray());
        Assert.True(session.Redo().IsSuccess);
        Assert.Empty(observed);
    }

    [Fact]
    public void Session_query_is_rejected_off_the_edit_write_thread()
    {
        var session = new MapEditSession(isWriteThread: () => false);
        Assert.Throws<InvalidOperationException>(() => session.QueryLocalRegions(Near));
    }

    static MapRegion Region(MapEditSession session, double x) => new(
        MapRegionId.New(), session.CurrentMap.Layers[2].LayerId, "区域", MapRegionKind.Generic,
        [new(x - 10, -10), new(x + 10, -10), new(x, 10)]);
}
