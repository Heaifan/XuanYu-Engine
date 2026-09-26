namespace XuanYu.Render.Abstractions;

public sealed record TerrainHeightfield
{
    public TerrainHeightfield(int width, int height, IReadOnlyList<double> elevationMeters)
        : this(width, height, elevationMeters, new bool[elevationMeters.Count],
            TerrainRenderMetadata.Empty, 1.0) { }

    public TerrainHeightfield(int width, int height, IReadOnlyList<double> elevationMeters,
        IReadOnlyList<bool> noDataMask, TerrainRenderMetadata metadata, double cellSizeMeters)
    {
        if (width < 2 || height < 2) throw new ArgumentOutOfRangeException(nameof(width));
        if (elevationMeters.Count != width * height)
            throw new ArgumentException("高程采样数量与 Heightfield 尺寸不一致。", nameof(elevationMeters));
        if (noDataMask.Count != elevationMeters.Count)
            throw new ArgumentException("NoData 掩码数量与高程样本不一致。", nameof(noDataMask));
        Width = width; Height = height; ElevationMeters = elevationMeters.ToArray();
        NoDataMask = noDataMask.ToArray(); Metadata = metadata; CellSizeMeters = cellSizeMeters;
    }

    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<double> ElevationMeters { get; }
    public IReadOnlyList<bool> NoDataMask { get; }
    public TerrainRenderMetadata Metadata { get; }
    public double CellSizeMeters { get; }
    public double ElevationAt(int row, int column) => ElevationMeters[row * Width + column];
}
