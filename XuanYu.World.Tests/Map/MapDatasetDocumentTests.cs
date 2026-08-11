using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapDatasetDocumentTests
{
    static MapDatasetDescriptor Descriptor() =>
        new("roads", MapDatasetTypes.Road, "data/roads.json");

    [Fact]
    public void New_document_has_the_approved_format_and_empty_features()
    {
        var document = MapDatasetDocument.CreateNew(Descriptor());
        Assert.Equal("xuanyu-map-dataset", document.Format);
        Assert.Equal(MapDatasetDocument.CurrentVersion, document.Version);
        Assert.Empty(document.Features);
        Assert.True(MapDatasetDocumentValidator.Validate(document).Succeeded);
    }

    [Fact]
    public void Json_contains_only_the_five_approved_keys()
    {
        var json = MapDatasetDocumentSerializer.Serialize(MapDatasetDocument.CreateNew(Descriptor()));
        using var parsed = JsonDocument.Parse(json);
        Assert.Equal(5, parsed.RootElement.EnumerateObject().Count());
        Assert.Contains("\"features\": []", json);
    }

    [Fact]
    public void Round_trip_preserves_empty_dataset_document()
    {
        var original = MapDatasetDocument.CreateNew(Descriptor());
        var parsed = MapDatasetDocumentSerializer.Deserialize(MapDatasetDocumentSerializer.Serialize(original));
        Assert.True(parsed.Succeeded);
        Assert.Equal(original.Format, parsed.Value!.Format);
        Assert.Equal(original.Id, parsed.Value.Id);
        Assert.Equal(original.Type, parsed.Value.Type);
        Assert.Empty(parsed.Value.Features);
    }

    [Fact]
    public void Unknown_fields_and_nonempty_features_are_rejected()
    {
        var unknown = MapDatasetDocumentSerializer.Deserialize("""
            {"format":"xuanyu-map-dataset","version":"0.1.0","id":"roads","type":"road","features":[],"geometry":{}}
            """);
        Assert.Equal("BrokenJson", unknown.ErrorCode);
        var nonempty = MapDatasetDocument.CreateNew(Descriptor()) with
        {
            Features = ImmutableArray.Create(JsonDocument.Parse("{}").RootElement.Clone())
        };
        Assert.Equal("InvalidFeatures", MapDatasetDocumentValidator.Validate(nonempty).ErrorCode);
    }

    [Fact]
    public void Invalid_identity_and_missing_features_fail_closed()
    {
        var bad = MapDatasetDocument.CreateNew(Descriptor()) with { Id = "Roads", Type = "building" };
        Assert.Equal("InvalidId", MapDatasetDocumentValidator.Validate(bad).ErrorCode);
        Assert.Equal("InvalidType", MapDatasetDocumentValidator.Validate(bad with { Id = "roads" }).ErrorCode);
        Assert.Equal("InvalidFeatures", MapDatasetDocumentValidator.Validate(bad with
        {
            Id = "roads", Type = "road", Features = default
        }).ErrorCode);
    }
}
