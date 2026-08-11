using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetContractTests
{
    static MapManifest Valid() => MapManifest.CreateNew("south-china", "华南") with
    {
        Datasets = ImmutableArray.Create(new MapDatasetDescriptor("roads", "road", "data/roads.json"))
    };

    [Fact]
    public void All_six_types_are_allowed_and_same_type_can_repeat()
    {
        var descriptors = MapDatasetTypes.All.Select((type, index) =>
            new MapDatasetDescriptor($"dataset-{index}", type, $"data/dataset-{index}.json"));
        var result = MapManifestValidator.Validate(Valid() with
        {
            Datasets = descriptors.Append(new MapDatasetDescriptor("roads-2", "road", "data/roads-2.json"))
                .ToImmutableArray()
        });
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Dataset_id_is_lowercase_and_case_insensitively_unique()
    {
        Assert.Equal("InvalidDatasetId", MapManifestValidator.Validate(Valid() with
        {
            Datasets = ImmutableArray.Create(new MapDatasetDescriptor("Roads.Data", "road", "data/x.json"))
        }).ErrorCode);
        Assert.Equal("DuplicateDatasetId", MapManifestValidator.Validate(Valid() with
        {
            Datasets = ImmutableArray.Create(
                new MapDatasetDescriptor("roads", "road", "data/a.json"),
                new MapDatasetDescriptor("roads", "river", "data/b.json"))
        }).ErrorCode);
    }

    [Fact]
    public void Unknown_types_and_unsafe_sources_are_rejected()
    {
        Assert.Equal("InvalidDatasetType", MapManifestValidator.Validate(Valid() with
        {
            Datasets = ImmutableArray.Create(new MapDatasetDescriptor("x", "building", "data/x.json"))
        }).ErrorCode);
        foreach (var source in new[] { "C:/x.json", "/x.json", "data/../x.json", "data\\x.json" })
            Assert.Equal("InvalidDatasetSource", MapManifestValidator.Validate(Valid() with
            {
                Datasets = ImmutableArray.Create(new MapDatasetDescriptor("x", "road", source))
            }).ErrorCode);
    }

    [Fact]
    public void Source_resolution_stays_under_map_root()
    {
        Assert.True(MapDatasetPathPolicy.TryResolve("E:/maps/south", "data/roads.json", out var path));
        Assert.EndsWith("data" + Path.DirectorySeparatorChar + "roads.json", path);
        Assert.False(MapDatasetPathPolicy.TryResolve("E:/maps/south", "data/../roads.json", out _));
    }
}
