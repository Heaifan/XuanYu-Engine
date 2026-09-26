namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainRenderExtent(double West, double South, double East, double North);

public readonly record struct TerrainRenderMetadata(
    int Width,
    int Height,
    double ResolutionX,
    double ResolutionY,
    double MinElevation,
    double MaxElevation,
    double? NoData,
    int NoDataCount,
    TerrainRenderExtent WorldExtent)
{
    public static TerrainRenderMetadata Empty => new(0, 0, 0, 0, 0, 0, null, 0, default);
}
