using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1-F1：默认地图工厂合同（完整聚合 + DTO 默认值一致）。
public sealed class MapDefaultMapTests
{
    [Fact]
    public void CreateDefault_produces_complete_aggregate()
    {
        var map = MapDefaultDefinition.CreateDefault();
        Assert.Equal("未命名地图", map.DisplayName);
        Assert.Equal(10000.0, map.SizeMeters.Width);
        Assert.Equal(10000.0, map.SizeMeters.Depth);
        Assert.Equal(MapSurfaceKinds.Flat, map.Surface.Kind);
        Assert.True(map.MapId.IsValid);
        Assert.Equal(MapCoordinateSystem.ZUpMeter, map.CoordinateSystem);
        Assert.Equal(2, map.Layers.Length);
        Assert.True(map.Regions.IsEmpty);
    }

    [Fact]
    public void CreateDefault_passes_strict_validation()
    {
        var result = MapDefinitionValidator.Validate(MapDefaultDefinition.CreateDefault());
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void CreateNew_dto_defaults_to_10km_flat()
    {
        var doc = MapDocument.CreateNew("测试战场");
        Assert.Equal(10000.0, doc.SizeMeters.Width);
        Assert.Equal(10000.0, doc.SizeMeters.Depth);
        Assert.Equal(MapSurfaceKinds.Flat, doc.Surface.Kind);
    }

    [Fact]
    public void CreateNew_accepts_r2_large_size()
    {
        var doc = MapDocument.CreateNew("测试战场", 20000.0, 8000.0);
        Assert.True(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void CreateNew_keeps_explicit_surface_contract()
    {
        var doc = MapDocument.CreateNew("测试战场", 2000.0, 2000.0,
            new MapSurfaceDefinition(MapSurfaceKinds.GentleHillsV1, 0.0, 12.0, 400.0, 1));
        Assert.Equal(MapSurfaceKinds.GentleHillsV1, doc.Surface.Kind);
        Assert.True(MapDocumentValidator.Validate(doc).Succeeded);
    }
}
