namespace XuanYu.World.Geo;

public static class GeographicCoordinateTransform
{
    public static GeographicPosition ToCanonical(SourceCoordinate source)
    {
        if (source.Crs != CoordinateReferenceSystem.Wgs84)
            throw new NotSupportedException($"Source CRS '{source.Crs}' is not supported in Terrain GEO-R0.");
        return new GeographicPosition(source.Latitude, source.Longitude, source.ElevationMeters);
    }
}
