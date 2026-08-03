using System.Collections.Immutable;
using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：坐标合同 / 图层引用 / schema / 名称校验。
public sealed class MapCoordinateValidationTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

    [Fact]
    public void Wrong_up_axis_rejected()
    {
        var doc = Valid() with
        {
            CoordinateSystem = new MapCoordinateSystem("meter", "Y", new MapVector3(0, 0, 0))
        };
        var result = MapDocumentValidator.Validate(doc);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidCoordinateSystem", result.ErrorCode);
    }

    [Fact]
    public void Non_meter_unit_rejected()
    {
        var doc = Valid() with
        {
            CoordinateSystem = new MapCoordinateSystem("feet", "Z", new MapVector3(0, 0, 0))
        };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void Non_zero_origin_rejected()
    {
        var doc = Valid() with
        {
            CoordinateSystem = new MapCoordinateSystem("meter", "Z", new MapVector3(1, 0, 0))
        };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void Null_coordinate_system_rejected()
    {
        var doc = Valid() with { CoordinateSystem = null! };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void Non_empty_layer_references_rejected()
    {
        var doc = Valid() with { LayerReferences = new[] { "L1" }.ToImmutableArray() };
        var result = MapDocumentValidator.Validate(doc);
        Assert.False(result.Succeeded);
        Assert.Equal("NonEmptyLayerReferences", result.ErrorCode);
    }

    [Fact]
    public void Missing_layer_references_rejected()
    {
        var doc = Valid() with { LayerReferences = default };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void Wrong_schema_version_rejected()
    {
        var doc = Valid() with { SchemaVersion = 2 };
        var result = MapDocumentValidator.Validate(doc);
        Assert.False(result.Succeeded);
        Assert.Equal("UnsupportedSchema", result.ErrorCode);
    }

    [Fact]
    public void Blank_name_rejected()
    {
        var doc = Valid() with { Name = "  " };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }
}
