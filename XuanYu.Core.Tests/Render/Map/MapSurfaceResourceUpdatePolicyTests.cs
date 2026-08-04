using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R2-D3-A1：地图 GPU 资源更新决策（纯策略）——旧序号拒绝、同键不重建、异键重建。
public sealed class MapSurfaceResourceUpdatePolicyTests
{
    static MapRenderSnapshot Snap(long seq, double width = 10000, double baseHeight = 0) => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", width, 10000,
        MapSurfaceKind.Flat, baseHeight, 0, 1, 1, seq);

    [Fact]
    public void First_snapshot_always_recreates()
    {
        var update = MapSurfaceResourceUpdatePolicy.Decide(Snap(0), long.MinValue, null);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.Equal("21e4a2d34d4a4a1eb2539eac76d412a8", update.Key.MapId);
    }

    [Fact]
    public void Stale_sequence_is_rejected()
    {
        var update = MapSurfaceResourceUpdatePolicy.Decide(Snap(4), lastConsumedSequence: 5,
            MapSurfaceResourceKey.From(Snap(5)));

        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, update.Kind);
    }

    [Fact]
    public void Same_key_does_not_rebuild()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));

        var update = MapSurfaceResourceUpdatePolicy.Decide(Snap(2), 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.NoRebuild, update.Kind);
    }

    [Fact]
    public void Resize_triggers_recreate()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));

        var update = MapSurfaceResourceUpdatePolicy.Decide(Snap(2, width: 20000), 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
    }

    [Fact]
    public void Base_height_triggers_recreate()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));

        var update = MapSurfaceResourceUpdatePolicy.Decide(Snap(2, baseHeight: 100), 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
    }

    [Fact]
    public void New_map_id_triggers_recreate()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));
        var next = Snap(2) with { MapId = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb" };

        var update = MapSurfaceResourceUpdatePolicy.Decide(next, 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
    }

    [Fact]
    public void Empty_map_after_map_recreates_to_clear()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));
        var empty = MapRenderSnapshot.Empty with { SourceChangeSequence = 1 };

        var update = MapSurfaceResourceUpdatePolicy.Decide(empty, 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.False(update.Key.IsVisible);
    }

    [Fact]
    public void Equal_sequence_equal_key_is_no_rebuild()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));

        var update = MapSurfaceResourceUpdatePolicy.Decide(Snap(1), 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.NoRebuild, update.Kind);
    }
}
