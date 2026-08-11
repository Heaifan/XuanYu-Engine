using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-DOC-A-R1：序列化、UTF-8 与 Round-trip 合同 C01～C05、D01～D05。
public sealed class MapManifestSerializationTests
{
    static MapManifest Valid() => MapManifest.CreateNew("south-china", "华南地图");

    [Fact]
    public void Json_uses_the_approved_keys_and_unicode()
    {
        var json = MapManifestSerializer.Serialize(Valid());

        Assert.Contains("\"coordinate_system\"", json);
        Assert.Contains("\"datasets\": []", json);
        Assert.Contains("华南地图", json);
        Assert.DoesNotContain("CoordinateSystem", json);
    }

    [Fact]
    public void Serialize_deserialize_round_trip_preserves_domain_values()
    {
        var original = Valid();
        var parsed = MapManifestSerializer.Deserialize(MapManifestSerializer.Serialize(original));

        Assert.True(parsed.Succeeded);
        Assert.Equal(original.Format, parsed.Value!.Format);
        Assert.Equal(original.Version, parsed.Value.Version);
        Assert.Equal(original.Id, parsed.Value.Id);
        Assert.Equal(original.Name, parsed.Value.Name);
        Assert.Equal(original.CoordinateSystem, parsed.Value.CoordinateSystem);
        Assert.Empty(parsed.Value.Datasets);
        Assert.Empty(parsed.Value.Assets);
        Assert.True(MapManifestValidator.Validate(parsed.Value).Succeeded);
    }

    [Fact]
    public void Unknown_fields_and_wrong_container_types_are_rejected()
    {
        var unknown = MapManifestSerializer.Deserialize("""
            {"format":"xuanyu-map","version":"0.1.0","id":"x","name":"x","coordinate_system":{"type":"local_cartesian","unit":"meter"},"datasets":[],"assets":[],"camera":{}}
            """);
        var wrongType = MapManifestSerializer.Deserialize("""
            {"format":"xuanyu-map","version":"0.1.0","id":"x","name":"x","coordinate_system":{"type":"local_cartesian","unit":"meter"},"datasets":{},"assets":[]}
            """);

        Assert.Equal("BrokenJson", unknown.ErrorCode);
        Assert.Equal("BrokenJson", wrongType.ErrorCode);
    }
}
