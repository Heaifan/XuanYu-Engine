using XuanYu.Core.Math;
using XuanYu.World.Geo;

namespace XuanYu.World.Tests.Geo;

public sealed class GeographicWorldMappingTests
{
    static readonly GeographicWorldMapping Mapping = new(new(25, 121, 100));

    [Fact]
    public void Longitude_maps_to_world_x_and_latitude_to_world_y()
    {
        var world = Mapping.ToWorld(new(25.001, 121.001, 100));

        Assert.True(world.X > 0);
        Assert.True(world.Y > 0);
        Assert.Equal(0, world.Z, precision: 6);
    }

    [Fact]
    public void Longitude_and_latitude_are_not_swapped_or_mirrored()
    {
        var east = Mapping.ToWorld(new(25, 121.001, 100));
        var north = Mapping.ToWorld(new(25.001, 121, 100));
        var west = Mapping.ToWorld(new(25, 120.999, 100));
        var south = Mapping.ToWorld(new(24.999, 121, 100));

        Assert.True(east.X > 0 && west.X < 0);
        Assert.True(north.Y > 0 && south.Y < 0);
        Assert.Equal(0, east.Y, precision: 6);
        Assert.Equal(0, north.X, precision: 6);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123.45)]
    [InlineData(-42.5)]
    public void Elevation_remains_meter_scale(double elevation)
    {
        var world = Mapping.ToWorld(new(25, 121, elevation));

        Assert.Equal(elevation - 100, world.Z, precision: 6);
    }

    [Fact]
    public void Mapping_is_deterministic_and_supports_reverse_mapping()
    {
        var input = new GeographicPosition(24.9995, 121.0005, 37.25);
        var first = Mapping.ToWorld(input);
        var second = Mapping.ToWorld(input);
        var roundTrip = Mapping.ToGeographic(first);

        Assert.Equal(first, second);
        Assert.Equal(input.Latitude, roundTrip.Latitude, precision: 9);
        Assert.Equal(input.Longitude, roundTrip.Longitude, precision: 9);
        Assert.Equal(input.ElevationMeters, roundTrip.ElevationMeters, precision: 9);
    }

    [Fact]
    public void Unsupported_source_crs_has_explicit_error_path()
    {
        var source = new SourceCoordinate(CoordinateReferenceSystem.WebMercator, 0, 0, 0);

        var error = Assert.Throws<NotSupportedException>(() =>
            GeographicCoordinateTransform.ToCanonical(source));

        Assert.Contains("WebMercator", error.Message);
    }

    [Fact]
    public void Wgs84_source_converts_without_changing_geographic_facts()
    {
        var source = new SourceCoordinate(CoordinateReferenceSystem.Wgs84, 25, 121, -12.5);

        var canonical = GeographicCoordinateTransform.ToCanonical(source);

        Assert.Equal(new GeographicPosition(25, 121, -12.5), canonical);
    }
}
