using XuanYu.Core.Map;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;
// MAP-A-R1-D3：World 地图状态所有者——加载/切换/卸载/查询/渲染快照。
public sealed class WorldMapStateOwnerTests
{
    static WorldMapState State(string id = "21e4a2d34d4a4a1eb2539eac76d412a8") => new(
        id, "TestBattlefield", 2000.0, 2000.0,
        MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1);

    [Fact]
    public void Initial_state_has_no_map()
    {
        var owner = new WorldMapStateOwner();
        Assert.False(owner.HasMap);
        Assert.Null(owner.CurrentMap);
        Assert.False(owner.TryGetSurfaceHeight(0, 0, out _));
    }
    [Fact]
    public void Load_enables_query()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(State());
        Assert.True(owner.HasMap);
        Assert.True(owner.TryGetSurfaceHeight(0, 0, out var z));
        Assert.Equal(State().SampleHeight(0, 0), z);
    }

    [Fact]
    public void Unload_clears_everything()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(State());
        owner.Unload();
        Assert.False(owner.HasMap);
        Assert.Null(owner.CurrentMap);
        Assert.False(owner.TryGetSurfaceHeight(0, 0, out _));
    }
    [Fact]
    public void Switch_replaces_old_state_completely()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(State("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        var replacement = State("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb") with
        {
            WidthMeters = 4000.0,
            BaseHeightMeters = 30.0
        };
        owner.Load(replacement);

        Assert.Equal("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", owner.CurrentMap!.MapId);
        Assert.Equal(4000.0, owner.CurrentMap.WidthMeters);
        Assert.Equal(30.0, owner.CurrentMap.BaseHeightMeters);
        Assert.True(owner.TryGetSurfaceHeight(1500, 0, out var z));
        Assert.Equal(replacement.SampleHeight(1500, 0), z);
    }
    [Fact]
    public void Empty_snapshot_without_map()
    {
        var owner = new WorldMapStateOwner();
        var snap = owner.BuildRenderSnapshot();
        Assert.False(snap.HasMap);
        Assert.Equal("", snap.MapId);
    }
    [Fact]
    public void Snapshot_carries_map_surface_data()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(State());
        var snap = owner.BuildRenderSnapshot();
        Assert.True(snap.HasMap);
        Assert.Equal("TestBattlefield", snap.Name);
        Assert.Equal(2000.0, snap.WidthMeters);
        Assert.Equal(2000.0, snap.DepthMeters);
        Assert.Equal(MapSurfaceKind.GentleHillsV1, snap.SurfaceKind);
        Assert.Equal(12.0, snap.AmplitudeMeters);
        Assert.Equal(400.0, snap.WavelengthMeters);
        Assert.Equal(1, snap.Seed);
    }

    [Fact]
    public void Snapshot_after_unload_is_empty()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(State());
        owner.Unload();
        Assert.False(owner.BuildRenderSnapshot().HasMap);
    }

    [Fact]
    public void Switch_does_not_leak_old_snapshot()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(State("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        owner.Load(State("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"));
        var snap = owner.BuildRenderSnapshot();
        Assert.Equal("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", snap.MapId);
    }
}
