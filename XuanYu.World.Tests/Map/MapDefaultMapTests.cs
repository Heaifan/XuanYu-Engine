using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1：默认地图工厂合同（10 km × 10 km、Flat、中心原点、稳定 ID）。
public sealed class MapDefaultMapTests
{
    [Fact]
    public void CreateDefault_produces_unnamed_10km_flat_map()
    {
        var doc = MapDocument.CreateDefault();
        Assert.Equal("未命名地图", doc.Name);
        Assert.Equal(10000.0, doc.SizeMeters.Width);
        Assert.Equal(10000.0, doc.SizeMeters.Depth);
        Assert.Equal(MapSurfaceKinds.Flat, doc.Surface.Kind);
        Assert.True(doc.MapId.IsValid);
        Assert.Equal(MapCoordinateSystem.ZUpMeter, doc.CoordinateSystem);
    }

    [Fact]
    public void CreateDefault_passes_strict_validation()
    {
        var result = MapDocumentValidator.Validate(MapDocument.CreateDefault());
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void CreateNew_defaults_to_10km_flat()
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
