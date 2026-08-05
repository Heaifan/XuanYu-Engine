using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1-F1：地图聚合验证（尺寸/坐标/地表/图层/区域组合入口）。
public sealed class MapDefinitionTests
{
    static MapDefinition Valid() => MapDefaultDefinition.CreateDefault();

    [Fact]
    public void Default_map_passes_validation() =>
        Assert.True(MapDefinitionValidator.Validate(Valid()).Succeeded);

    [Fact]
    public void Null_map_rejected()
    {
        var result = MapDefinitionValidator.Validate(null);
        Assert.False(result.Succeeded);
        Assert.Equal("NullMap", result.ErrorCode);
    }

    [Fact]
    public void Blank_name_rejected()
    {
        var result = MapDefinitionValidator.Validate(Valid() with { DisplayName = " " });
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidMapName", result.ErrorCode);
    }

    [Theory]
    [InlineData(99.9)]
    [InlineData(1000000.1)]
    public void Out_of_range_size_rejected(double width)
    {
        var map = Valid() with { SizeMeters = new MapSize(width, 10000.0) };
        var result = MapDefinitionValidator.Validate(map);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidSize", result.ErrorCode);
    }

    [Fact]
    public void Non_zup_coordinate_system_rejected()
    {
        var yUp = new MapCoordinateSystem("meter", "Y", new MapVector3(0, 0, 0));
        var result = MapDefinitionValidator.Validate(Valid() with { CoordinateSystem = yUp });
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidCoordinateSystem", result.ErrorCode);
    }

    [Fact]
    public void Unknown_surface_kind_rejected()
    {
        var surface = Valid().Surface with { Kind = "Volcano" };
        var result = MapDefinitionValidator.Validate(Valid() with { Surface = surface });
        Assert.False(result.Succeeded);
        Assert.Equal("UnknownSurfaceKind", result.ErrorCode);
    }

    [Fact]
    public void Invalid_region_in_aggregate_rejected()
    {
        var map = Valid();
        var collinear = ImmutableArray.Create(
            new MapPoint(0, 0), new MapPoint(10, 0), new MapPoint(20, 0));
        var region = new MapRegion(
            MapRegionId.New(), map.Layers[2].LayerId, "坏区域",
            MapRegionKind.Generic, collinear);
        var result = MapDefinitionValidator.Validate(map with { Regions = [region] });
        Assert.False(result.Succeeded);
        Assert.Equal("ZeroAreaRegion", result.ErrorCode);
    }
}
