namespace XuanYu.World.Geo;

public readonly record struct GeographicPosition
{
    public GeographicPosition(double latitude, double longitude, double elevationMeters)
    {
        if (!double.IsFinite(latitude) || latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude));
        if (!double.IsFinite(longitude) || longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude));
        if (!double.IsFinite(elevationMeters))
            throw new ArgumentOutOfRangeException(nameof(elevationMeters));
        Latitude = latitude;
        Longitude = longitude;
        ElevationMeters = elevationMeters;
    }

    public double Latitude { get; }
    public double Longitude { get; }
    public double ElevationMeters { get; }
}
