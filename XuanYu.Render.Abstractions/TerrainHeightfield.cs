namespace XuanYu.Render.Abstractions;

public sealed record TerrainHeightfield
{
    public TerrainHeightfield(int width, int height, IReadOnlyList<double> elevationMeters)
    {
        if (width < 2 || height < 2) throw new ArgumentOutOfRangeException(nameof(width));
        if (elevationMeters.Count != width * height)
            throw new ArgumentException("高程采样数量与 Heightfield 尺寸不一致。", nameof(elevationMeters));
        Width = width; Height = height; ElevationMeters = elevationMeters;
    }

    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<double> ElevationMeters { get; }
    public double ElevationAt(int row, int column) => ElevationMeters[row * Width + column];
}
