namespace XuanYu.World.Terrain.Source;

public sealed class TerrainSourceData
{
    public TerrainSourceData(TerrainSourceRaster raster, string sourceCrs,
        TerrainGeographicBounds geographicBounds, TerrainResolution resolution,
        IReadOnlyDictionary<string, string> metadata)
    {
        Raster = raster;
        SourceCrs = string.IsNullOrWhiteSpace(sourceCrs) ? "Unknown" : sourceCrs;
        GeographicBounds = geographicBounds;
        Resolution = resolution;
        Metadata = new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase);
    }

    public TerrainSourceRaster Raster { get; }
    public string SourceCrs { get; }
    public TerrainGeographicBounds GeographicBounds { get; }
    public TerrainResolution Resolution { get; }
    public IReadOnlyDictionary<string, string> Metadata { get; }
}
