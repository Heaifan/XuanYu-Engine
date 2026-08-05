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
    public void From_aggregate_projects_to_world_state()
    {
        Assert.True(MapId.TryParse("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", out var mapId));
        var map = new MapDefinition(
            mapId, "战场",
            new MapSize(4000, 3000),
            MapCoordinateSystem.ZUpMeter,
            new MapSurfaceDefinition(MapSurfaceKinds.GentleHillsV1, 12, 5, 200, 7),
            [new MapLayer(MapLayerId.New(), "地面", 0, MapLayerKind.Ground)],
            []);
        var state = WorldMapState.From(map);

        Assert.Equal("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", state.MapId);
        Assert.Equal("战场", state.Name);
        Assert.Equal(4000.0, state.WidthMeters);
        Assert.Equal(3000.0, state.DepthMeters);
        Assert.Equal(MapSurfaceKind.GentleHillsV1, state.SurfaceKind);
        Assert.Equal(12.0, state.BaseHeightMeters);
        Assert.Equal(5.0, state.AmplitudeMeters);
        Assert.Equal(200.0, state.WavelengthMeters);
        Assert.Equal(7, state.Seed);
        Assert.True(state.Contains(1500, 1000));
        Assert.False(state.Contains(2500, 0));
    }
}
