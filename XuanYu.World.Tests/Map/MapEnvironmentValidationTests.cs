using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：环境定义与参数校验。
public sealed class MapEnvironmentValidationTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

    [Fact]
    public void Valid_environment_passes()
    {
        Assert.True(MapDocumentValidator.Validate(Valid()).Succeeded);
    }

    [Fact]
    public void Zero_sun_direction_rejected()
    {
        var doc = Valid() with
        {
            Environment = new MapEnvironmentDefinition(
                "ClearDayV1", new MapVector3(0, 0, 0), 1.0, 0.35)
        };
        var result = MapDocumentValidator.Validate(doc);
        Assert.False(result.Succeeded);
        Assert.Equal("InvalidEnvironment", result.ErrorCode);
    }

    [Fact]
    public void Non_finite_sun_direction_rejected()
    {
        var doc = Valid() with
        {
            Environment = new MapEnvironmentDefinition(
                "ClearDayV1", new MapVector3(double.NaN, 0, 1), 1.0, 0.35)
        };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void Negative_or_non_finite_sun_intensity_rejected(double intensity)
    {
        var doc = Valid() with
        {
            Environment = new MapEnvironmentDefinition(
                "ClearDayV1", new MapVector3(-0.35, -0.55, 0.75), intensity, 0.35)
        };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    public void Negative_or_non_finite_ambient_rejected(double ambient)
    {
        var doc = Valid() with
        {
            Environment = new MapEnvironmentDefinition(
                "ClearDayV1", new MapVector3(-0.35, -0.55, 0.75), 1.0, ambient)
        };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }

    [Fact]
    public void Unknown_sky_preset_rejected()
    {
        var doc = Valid() with
        {
            Environment = new MapEnvironmentDefinition(
                "NightV1", new MapVector3(-0.35, -0.55, 0.75), 1.0, 0.35)
        };
        Assert.False(MapDocumentValidator.Validate(doc).Succeeded);
    }
}
