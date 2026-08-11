using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-DOC-A-R1：Manifest 校验合同 B01～B07。
public sealed class MapManifestValidationTests
{
    static MapManifest Valid() => MapManifest.CreateNew("south-china", "华南");

    [Fact]
    public void Format_and_version_are_fail_closed()
    {
        Assert.Equal("InvalidFormat", MapManifestValidator.Validate(Valid() with { Format = "other" }).ErrorCode);
        Assert.Equal("UnsupportedVersion", MapManifestValidator.Validate(Valid() with { Version = "9.0.0" }).ErrorCode);
    }

    [Fact]
    public void Id_and_name_are_validated()
    {
        Assert.Equal("InvalidId", MapManifestValidator.Validate(Valid() with { Id = "South China" }).ErrorCode);
        Assert.Equal("InvalidName", MapManifestValidator.Validate(Valid() with { Name = " " }).ErrorCode);
    }

    [Fact]
    public void Coordinate_system_is_validated()
    {
        var bad = Valid() with
        {
            CoordinateSystem = new MapManifestCoordinateSystem("geographic", "degree")
        };
        Assert.Equal("InvalidCoordinateSystem", MapManifestValidator.Validate(bad).ErrorCode);
    }

    [Fact]
    public void Dataset_and_asset_containers_must_be_present_arrays()
    {
        Assert.Equal("InvalidDatasets", MapManifestValidator.Validate(Valid() with { Datasets = default }).ErrorCode);
        Assert.Equal("InvalidAssets", MapManifestValidator.Validate(Valid() with { Assets = default }).ErrorCode);
    }

    [Fact]
    public void Container_items_are_preserved_without_registry_semantics()
    {
        var manifest = Valid() with
        {
            Datasets = ImmutableArray.Create(new MapDatasetDescriptor("future", "region", "data/future.json"))
        };
        Assert.True(MapManifestValidator.Validate(manifest).Succeeded);
        Assert.Equal("future", manifest.Datasets[0].Id);
    }
}
