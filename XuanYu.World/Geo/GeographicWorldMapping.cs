using XuanYu.Core.Math;

namespace XuanYu.World.Geo;

public sealed class GeographicWorldMapping
{
    const double EarthRadiusMeters = 6378137.0;
    readonly GeographicPosition _origin;
    readonly double _metersPerDegreeLatitude;
    readonly double _metersPerDegreeLongitude;

    public GeographicWorldMapping(GeographicPosition origin)
    {
        if (origin.Latitude is <= -90 or >= 90)
            throw new ArgumentOutOfRangeException(nameof(origin), "World mapping origin cannot be at a geographic pole.");
        _origin = origin;
        var latitudeRadians = DegreesToRadians(origin.Latitude);
        _metersPerDegreeLatitude = EarthRadiusMeters * Math.PI / 180.0;
        _metersPerDegreeLongitude = _metersPerDegreeLatitude * Math.Cos(latitudeRadians);
    }

    public Vector3d ToWorld(GeographicPosition position) => new(
        (position.Longitude - _origin.Longitude) * _metersPerDegreeLongitude,
        (position.Latitude - _origin.Latitude) * _metersPerDegreeLatitude,
        position.ElevationMeters - _origin.ElevationMeters);

    public GeographicPosition ToGeographic(Vector3d world) => new(
        _origin.Latitude + world.Y / _metersPerDegreeLatitude,
        _origin.Longitude + world.X / _metersPerDegreeLongitude,
        _origin.ElevationMeters + world.Z);

    static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
