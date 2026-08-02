using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：地表定义与参数校验。
public sealed class MapSurfaceValidationTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

    [Theory]
    [InlineData("Flat")]
    [InlineData("GentleHillsV1")]
    public void Known_surface_kinds_are_valid(string kind)
    {
        var doc = Valid() with { Surface = new MapSurfaceDefinition(kind, 0, 12, 400, 1) };
        Assert.True(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void Unknown_surface_kind_rejected()
    {
        var doc = Valid() with { Surface = new MapSurfaceDefinition("Volcano", 0, 12, 400, 1) };
        var result = MapDocumentValidator.Validate(doc);
        Assert.False(result.Succeeded);
        Assert.Equal("UnknownSurfaceKind", result.ErrorCode);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Invalid_amplitude_rejected(double amplitude)
    {
        var doc = Valid() with { Surface = new MapSurfaceDefinition("Flat", 0, amplitude, 400, 1) };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    public void Invalid_wavelength_rejected(double wavelength)
    {
        var doc = Valid() with { Surface = new MapSurfaceDefinition("Flat", 0, 12, wavelength, 1) };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }
}
