namespace XuanYu.World.Geo;

public enum CoordinateReferenceSystem
{
    Wgs84,
    WebMercator
}

public readonly record struct SourceCoordinate(
    CoordinateReferenceSystem Crs,
    double Latitude,
    double Longitude,
    double ElevationMeters);
