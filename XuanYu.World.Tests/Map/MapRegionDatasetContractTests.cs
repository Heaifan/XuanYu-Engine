using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class MapRegionDatasetContractTests
{
    [Fact]
    public void Version_rules_keep_legacy_empty_and_allow_region_features_at_02()
    {
        Assert.True(MapDatasetDocumentValidator.Validate(Document("0.1.0", [])).Succeeded);
        Assert.False(MapDatasetDocumentValidator.Validate(Document("0.1.0", [Feature()])).Succeeded);
        Assert.True(MapDatasetDocumentValidator.Validate(Document("0.2.0", [Feature()])).Succeeded);
    }

    [Fact]
    public void Region_features_reject_unknown_fields_and_repeated_tail()
    {
        var unknown = Json("""{"id":"11111111111111111111111111111111","geometry":{"type":"polygon","points":[{"x":0,"y":0},{"x":2,"y":0},{"x":0,"y":2}]},"properties":{"name":"A","kind":"generic"},"extra":1}""");
        var repeated = Json("""{"id":"11111111111111111111111111111111","geometry":{"type":"polygon","points":[{"x":0,"y":0},{"x":2,"y":0},{"x":0,"y":0}]},"properties":{"name":"A","kind":"generic"}}""");
        Assert.False(MapDatasetDocumentValidator.Validate(Document("0.2.0", [unknown])).Succeeded);
        Assert.False(MapDatasetDocumentValidator.Validate(Document("0.2.0", [repeated])).Succeeded);
    }

    static MapDatasetDocument Document(string version, ImmutableArray<JsonElement> features) =>
        new(MapDatasetDocument.CurrentFormat, version, "region-abc", MapDatasetTypes.Region, features);
    static JsonElement Feature() => Json("""{"id":"11111111111111111111111111111111","geometry":{"type":"polygon","points":[{"x":0,"y":0},{"x":2,"y":0},{"x":0,"y":2}]},"properties":{"name":"A","kind":"generic"}}""");
    static JsonElement Json(string text) => JsonDocument.Parse(text).RootElement.Clone();
}
