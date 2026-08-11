using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

public sealed class MapRegionDatasetRuntimeTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-runtime-{Guid.NewGuid():N}");

    [Fact]
    public void Hydration_and_live_projection_keep_region_content_in_memory()
    {
        var manifest = Manifest();
        var first = Feature("11111111111111111111111111111111", 10);
        var second = Feature("22222222222222222222222222222222", 20);
        var bound = MapDatasetRegionBinding.Build(MapDefaultDefinition.CreateDefault(), manifest,
            [Document("region-a", first), Document("region-b", second)]);

        Assert.True(bound.Succeeded);
        Assert.Equal(2, bound.Value!.Regions.Length);
        var changed = manifest with { Datasets = manifest.Datasets.SetItem(0, manifest.Datasets[0] with { Name = "甲区" }),
            DatasetLayerStates = [new("region-b", false, true, 0), new("region-a", true, false, 1)] };
        var projected = MapDatasetRuntimeProjection.Apply(bound.Value, changed);

        Assert.True(projected.Succeeded);
        Assert.Equal(bound.Value.Regions, projected.Value!.Regions);
        var layer = projected.Value.Layers.Single(item => item.LayerId == MapDatasetLayerIdProjection.Project("region-a"));
        Assert.Equal("甲区", layer.DisplayName);
        Assert.Equal(3, projected.Value.Layers.Single(item => item.LayerId == MapDatasetLayerIdProjection.Project("region-b")).Order);
    }

    [Fact]
    public async Task Save_partitions_regions_upgrades_legacy_and_rolls_back_group_on_commit_failure()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var manifest = Manifest();
        var registry = new MapDatasetRegistry(path, manifest);
        var bound = MapDatasetRegionBinding.Build(MapDefaultDefinition.CreateDefault(), manifest,
            [Document("region-a"), Document("region-b")]).Value!;
        var map = bound with { Regions = [Region("region-a", 10), Region("region-b", 20)] };
        var storage = new MapDatasetStorageService();
        var firstPath = Path.Combine(_root, "data", "region-a.json");
        await storage.SaveAsync(firstPath, Document("region-a") with { Version = MapDatasetDocument.LegacyVersion });
        await storage.SaveAsync(Path.Combine(_root, "data", "region-b.json"), Document("region-b"));

        Assert.True((await registry.SaveRegionContentAsync(map)).Succeeded);
        var first = await storage.LoadAsync(firstPath, manifest.Datasets[0]);
        Assert.Equal(MapDatasetDocument.CurrentVersion, first.Document!.Version);
        Assert.Single(first.Document.Features);

        var stable = await File.ReadAllTextAsync(firstPath);
        File.Delete(Path.Combine(_root, "data", "region-b.json"));
        Directory.CreateDirectory(Path.Combine(_root, "data", "region-b.json"));
        var failed = await registry.SaveRegionContentAsync(map);
        Assert.False(failed.Succeeded);
        Assert.Equal(stable, await File.ReadAllTextAsync(firstPath));
    }

    static MapManifest Manifest() => MapManifest.CreateNew("map-a", "地图") with
    {
        Datasets = [new("region-a", "region", "data/region-a.json"), new("region-b", "region", "data/region-b.json")],
        DatasetLayerStates = [new("region-a", true, false, 0), new("region-b", true, false, 1)]
    };

    static MapDatasetDocument Document(string id, params System.Text.Json.JsonElement[] features) =>
        new(MapDatasetDocument.CurrentFormat, MapDatasetDocument.CurrentVersion, id, "region", features.ToImmutableArray());
    static System.Text.Json.JsonElement Feature(string id, double x) => MapRegionDatasetCodec.Write(new(
        MapRegionId.TryParse(id, out var regionId) ? regionId : throw new InvalidOperationException(), MapLayerId.New(), "区域",
        MapRegionKind.Generic, [new(x, 0), new(x + 1, 0), new(x, 1)]));
    static MapRegion Region(string datasetId, double x) => new(MapRegionId.New(), MapDatasetLayerIdProjection.Project(datasetId),
        "区域", MapRegionKind.Generic, [new(x, 0), new(x + 1, 0), new(x, 1)]);
    public void Dispose() { try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch (IOException) { } }
}
