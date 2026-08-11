using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetLayerStateTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-layer-state-{Guid.NewGuid():N}");

    [Fact]
    public void Legacy_manifest_without_layer_state_gets_defaults_without_requiring_save()
    {
        var parsed = MapManifestSerializer.Deserialize("""
            {"format":"xuanyu-map","version":"0.1.0","id":"map-a","name":"地图","coordinate_system":{"type":"local_cartesian","unit":"meter"},"datasets":[{"id":"road-a","type":"road","source":"data/road-a.json"}],"assets":[]}
            """);
        Assert.True(parsed.Succeeded);
        Assert.Equal(new DatasetLayerState("road-a", true, false, 0), parsed.Value!.DatasetLayerStates.Single());
    }

    [Fact]
    public void Validator_rejects_duplicate_dangling_and_non_contiguous_layer_state()
    {
        var data = ImmutableArray.Create(new MapDatasetDescriptor("road-a", "road", "data/road-a.json"));
        var valid = MapManifest.CreateNew("map-a", "地图") with { Datasets = data };
        Assert.False(MapManifestValidator.Validate(valid with { DatasetLayerStates = [] }).Succeeded);
        Assert.False(MapManifestValidator.Validate(valid with { DatasetLayerStates = [
            new("road-a", true, false, 1)] }).Succeeded);
        Assert.False(MapManifestValidator.Validate(valid with { DatasetLayerStates = [
            new("other", true, false, 0)] }).Succeeded);
    }

    [Fact]
    public async Task Working_promotion_preserves_layer_states_and_locked_unregister_fails_closed()
    {
        var working = new MapWorkingStorage(_root);
        var baseline = MapManifest.CreateNew("map-a", "地图");
        var path = (await working.EnsureAsync(baseline)).Value!;
        var registry = new MapDatasetRegistry(path, baseline);
        Assert.True((await registry.CreateAsync("road-a", "road")).Succeeded);
        Assert.True((await registry.CreateAsync("river-b", "river")).Succeeded);
        Assert.True(registry.UpdateLayerStates([
            new("river-b", true, true, 0), new("road-a", false, false, 1)]).Succeeded);
        Assert.False((await registry.UnregisterAsync("river-b")).Succeeded);
        var formal = Path.Combine(_root, "formal", "map.json");
        Assert.True((await working.PromoteAsync(formal, registry.CurrentManifest)).Succeeded);
        var reopened = await new MapManifestStorageService().LoadAsync(formal);
        Assert.Equal(["river-b", "road-a"], reopened.Value!.DatasetLayerStates.Select(item => item.DatasetId));
        Assert.True(reopened.Value.DatasetLayerStates[0].IsLocked);
        Assert.False(reopened.Value.DatasetLayerStates[1].IsVisible);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
