using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：.xymap 严格 JSON Round-trip 与确定性。
public sealed class MapJsonRoundTripTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield", 2000, 2000);

    const string ValidJson = """
        {
          "schemaVersion": 1,
          "mapId": "21e4a2d34d4a4a1eb2539eac76d412a8",
          "name": "TestBattlefield",
          "sizeMeters": { "width": 2000.0, "depth": 2000.0 },
          "coordinateSystem": { "unit": "meter", "upAxis": "Z", "origin": { "x": 0.0, "y": 0.0, "z": 0.0 } },
          "surface": { "kind": "GentleHillsV1", "baseHeightMeters": 0.0, "amplitudeMeters": 12.0, "wavelengthMeters": 400.0, "seed": 1 },
          "environment": { "skyPreset": "ClearDayV1", "sunDirection": { "x": -0.35, "y": -0.55, "z": 0.75 }, "sunIntensity": 1.0, "ambientIntensity": 0.35 },
          "layerReferences": []
        }
        """;
    [Fact]
    public void Round_trip_preserves_all_fields()
    {
        var doc = Valid();
        var loaded = MapJsonSerializer.Deserialize(MapJsonSerializer.Serialize(doc));
        Assert.True(loaded.Succeeded);
        Assert.Equal(doc, loaded.Value);
    }

    [Fact]
    public void Serialized_json_uses_fixed_lowercase_keys()
    {
        var json = MapJsonSerializer.Serialize(Valid());
        Assert.Contains("\"schemaVersion\": 1", json);
        Assert.Contains("\"mapId\":", json);
        Assert.Contains("\"sizeMeters\":", json);
        Assert.Contains("\"upAxis\": \"Z\"", json);
        Assert.Contains("\"layerReferences\": []", json);
        Assert.DoesNotContain("\"MapId\"", json);
        Assert.DoesNotContain("\"SizeMeters\"", json);
    }
    [Fact]
    public void Repeated_serialization_is_stable()
    {
        var doc = Valid();
        Assert.Equal(MapJsonSerializer.Serialize(doc), MapJsonSerializer.Serialize(doc));
    }

    [Fact]
    public void Chinese_map_name_survives_round_trip()
    {
        var doc = Valid() with { Name = "测试战场·缓丘" };
        var loaded = MapJsonSerializer.Deserialize(MapJsonSerializer.Serialize(doc));
        Assert.True(loaded.Succeeded);
        Assert.Equal("测试战场·缓丘", loaded.Value!.Name);
    }
    [Fact]
    public void Contract_json_deserializes_and_validates()
    {
        var loaded = MapJsonSerializer.Deserialize(ValidJson);
        Assert.True(loaded.Succeeded);
        Assert.True(MapDocumentValidator.Validate(loaded.Value!).Succeeded);
    }

    [Fact]
    public void Unknown_schema_version_is_rejected()
    {
        var json = ValidJson.Replace("\"schemaVersion\": 1", "\"schemaVersion\": 9");
        var loaded = MapJsonSerializer.Deserialize(json);
        Assert.True(loaded.Succeeded);
        Assert.False(MapDocumentValidator.Validate(loaded.Value!).Succeeded);
    }
    [Fact]
    public void Up_axis_y_is_rejected_without_conversion()
    {
        var json = ValidJson.Replace("\"upAxis\": \"Z\"", "\"upAxis\": \"Y\"");
        var loaded = MapJsonSerializer.Deserialize(json);
        Assert.True(loaded.Succeeded);
        Assert.False(MapDocumentValidator.Validate(loaded.Value!).Succeeded);
    }

    [Fact]
    public void Non_empty_layer_references_in_file_rejected()
    {
        var json = ValidJson.Replace("\"layerReferences\": []", "\"layerReferences\": [\"L1\"]");
        var loaded = MapJsonSerializer.Deserialize(json);
        Assert.True(loaded.Succeeded);
        Assert.False(MapDocumentValidator.Validate(loaded.Value!).Succeeded);
    }

    [Fact]
    public void Missing_layer_references_field_is_rejected()
    {
        var json = ValidJson.Replace("\"layerReferences\": []", "\"layerReferences\": null");
        var loaded = MapJsonSerializer.Deserialize(json);
        Assert.True(loaded.Succeeded);
        Assert.False(MapDocumentValidator.Validate(loaded.Value!).Succeeded);
    }
}
