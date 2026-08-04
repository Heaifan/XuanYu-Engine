using XuanYu.Core.Map;
using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D3：.xymap v1 DTO → 领域聚合桥接（场景 mapReference 保活链）与端到端查询一致。
public sealed class MapDocumentAggregateBridgeTests
{
    static MapDocument Doc() => MapDocument.CreateNew("TestBattlefield", 2000, 2000,
        new MapSurfaceDefinition(MapSurfaceKinds.GentleHillsV1, 0.0, 12.0, 400.0, 1));

    [Fact]
    public void Aggregate_preserves_document_fields()
    {
        var aggregate = MapDocumentAggregateBridge.ToAggregate(Doc());

        Assert.Equal(32, aggregate.MapId.Value.Length);
        Assert.Equal("TestBattlefield", aggregate.DisplayName);
        Assert.Equal(2000.0, aggregate.SizeMeters.Width);
        Assert.Equal(2000.0, aggregate.SizeMeters.Depth);
        Assert.Equal(MapSurfaceKinds.GentleHillsV1, aggregate.Surface.Kind);
        Assert.Equal(12.0, aggregate.Surface.AmplitudeMeters);
        Assert.Equal(400.0, aggregate.Surface.WavelengthMeters);
    }

    [Fact]
    public void Aggregate_adds_default_layers_and_passes_validation()
    {
        var aggregate = MapDocumentAggregateBridge.ToAggregate(Doc());

        Assert.Equal(2, aggregate.Layers.Length);
        Assert.Equal(MapLayerKind.Base, aggregate.Layers[0].Kind);
        Assert.Equal(MapLayerKind.Region, aggregate.Layers[1].Kind);
        Assert.True(aggregate.Regions.IsDefaultOrEmpty);
        Assert.True(MapDefinitionValidator.Validate(aggregate).Succeeded);
    }

    [Fact]
    public void Flat_document_bridges_to_flat_sampler()
    {
        var doc = Doc() with
        {
            Surface = new MapSurfaceDefinition(MapSurfaceKinds.Flat, 7.0, 0.0, 1.0, 0)
        };
        var state = WorldMapState.From(MapDocumentAggregateBridge.ToAggregate(doc));

        Assert.Equal(MapSurfaceKind.Flat, state.SurfaceKind);
        Assert.Equal(7.0, state.SampleHeight(123, -456));
    }

    [Fact]
    public void Aggregate_to_world_state_height_matches_document_surface()
    {
        var doc = Doc();
        var state = WorldMapState.From(MapDocumentAggregateBridge.ToAggregate(doc));
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
    public void Owner_end_to_end_load_aggregate_query()
    {
        var owner = new WorldMapStateOwner();
        owner.Load(WorldMapState.From(MapDocumentAggregateBridge.ToAggregate(Doc())));
        Assert.True(owner.TryGetSurfaceHeight(0, 0, out var z));
        Assert.Equal(owner.CurrentMap!.SampleHeight(0, 0), z);
        Assert.False(owner.TryGetSurfaceHeight(5000, 0, out _));
    }
}
