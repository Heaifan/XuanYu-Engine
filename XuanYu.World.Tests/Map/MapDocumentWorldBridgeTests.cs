using XuanYu.Core.Map;
using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D3：MapDocument → WorldMapState 桥接与端到端查询一致。
public sealed class MapDocumentWorldBridgeTests
{
    static MapDocument Doc() => MapDocument.CreateNew("TestBattlefield", 2000, 2000,
        new MapSurfaceDefinition(MapSurfaceKinds.GentleHillsV1, 0.0, 12.0, 400.0, 1));

    [Fact]
    public void Bridge_carries_all_fields()
    {
        var state = MapDocumentWorldBridge.ToWorldState(Doc());
        Assert.Equal(32, state.MapId.Length);
        Assert.Equal("TestBattlefield", state.Name);
        Assert.Equal(2000.0, state.WidthMeters);
        Assert.Equal(2000.0, state.DepthMeters);
        Assert.Equal(MapSurfaceKind.GentleHillsV1, state.SurfaceKind);
        Assert.Equal(12.0, state.AmplitudeMeters);
        Assert.Equal(400.0, state.WavelengthMeters);
        Assert.Equal(1, state.Seed);
    }

    [Fact]
    public void Flat_document_bridges_to_flat_sampler()
    {
        var doc = Doc() with
        {
            Surface = new MapSurfaceDefinition("Flat", 7.0, 0.0, 1.0, 0)
        };
        var state = MapDocumentWorldBridge.ToWorldState(doc);
        Assert.Equal(MapSurfaceKind.Flat, state.SurfaceKind);
        Assert.Equal(7.0, state.SampleHeight(123, -456));
    }

    [Fact]
    public void Bridge_height_matches_document_surface()
    {
        var doc = Doc();
        var state = MapDocumentWorldBridge.ToWorldState(doc);
        var expected = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1,
            doc.Surface.BaseHeightMeters,
            doc.Surface.AmplitudeMeters,
            doc.Surface.WavelengthMeters,
            doc.Surface.Seed,
            100.0, 200.0);
        Assert.Equal(expected, state.SampleHeight(100.0, 200.0));
    }

    [Fact]
    public void Owner_end_to_end_load_document_query()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(MapDocumentWorldBridge.ToWorldState(Doc()));
        Assert.True(owner.TryGetSurfaceHeight(0, 0, out var z));
        Assert.Equal(owner.CurrentMap!.SampleHeight(0, 0), z);
        Assert.False(owner.TryGetSurfaceHeight(5000, 0, out _));
    }
}
