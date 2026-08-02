using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：地图尺寸与坐标合同校验。
public sealed class MapSizeValidationTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

    [Fact]
    public void Valid_document_passes()
    {
        Assert.True(MapDocumentValidator.Validate(Valid()).Succeeded);
    }

    [Theory]
    [InlineData(100.0, 100.0)]
    [InlineData(10000.0, 10000.0)]
    public void Boundary_sizes_are_valid(double w, double d)
    {
        var doc = Valid() with { SizeMeters = new MapSize(w, d) };
        Assert.True(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Theory]
    [InlineData(99.9)]
    [InlineData(10000.1)]
    [InlineData(0.0)]
    [InlineData(-5.0)]
    public void Invalid_width_rejected(double width)
    {
        var doc = Valid() with { SizeMeters = new MapSize(width, 2000) };
        var result = MapDocumentValidator.Validate(doc);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidSize", result.ErrorCode);
        Assert.Contains("width", result.Message);
    }

    [Fact]
    public void Non_finite_size_rejected()
    {
        var nan = Valid() with { SizeMeters = new MapSize(double.NaN, 2000) };
        Assert.False(MapDocumentValidator.Validate(nan).Succeeded);
        var inf = Valid() with { SizeMeters = new MapSize(2000, double.PositiveInfinity) };
        Assert.False(MapDocumentValidator.Validate(inf).Succeeded);
    }
}
