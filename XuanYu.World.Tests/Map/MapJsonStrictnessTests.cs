using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：严格 JSON 拒绝路径（大小写 / 未知字段 / 类型 / 损坏）。
public sealed class MapJsonStrictnessTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

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
    public void Wrong_case_key_is_rejected()
    {
        var loaded = MapJsonSerializer.Deserialize(ValidJson.Replace("\"mapId\":", "\"MapId\":"));
        Assert.False(loaded.Succeeded);
        Assert.Equal("BrokenJson", loaded.ErrorCode);
    }

    [Fact]
    public void Unknown_field_is_rejected()
    {
        var json = ValidJson.Replace("\"layerReferences\": []", "\"layerReferences\": [], \"extra\": 1");
        Assert.False(MapJsonSerializer.Deserialize(json).Succeeded);
    }

    [Fact]
    public void Missing_required_field_is_rejected_by_validation()
    {
        var json = ValidJson.Replace(
            "\"sizeMeters\": { \"width\": 2000.0, \"depth\": 2000.0 },", "");
        var loaded = MapJsonSerializer.Deserialize(json);
        Assert.True(loaded.Succeeded, "缺字段由验证器拒绝而非解析失败");
        Assert.False(MapDocumentValidator.Validate(loaded.Value!).Succeeded);
    }

    [Fact]
    public void Wrong_type_field_is_rejected()
    {
        var json = ValidJson.Replace("\"schemaVersion\": 1", "\"schemaVersion\": \"one\"");
        Assert.False(MapJsonSerializer.Deserialize(json).Succeeded);
    }

    [Fact]
    public void Broken_json_is_rejected()
    {
        Assert.False(MapJsonSerializer.Deserialize("{ not json").Succeeded);
        Assert.False(MapJsonSerializer.Deserialize("").Succeeded);
    }
}
