using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain;

public sealed record TerrainMetadata(
    int Width,
    int Height,
    TerrainResolution Resolution,
    double MinElevation,
    double MaxElevation,
    double? NoData,
    int NoDataCount,
    TerrainGeographicBounds WorldExtent)
{
    public double ResolutionX => Resolution.X;
    public double ResolutionY => Resolution.Y;
}
