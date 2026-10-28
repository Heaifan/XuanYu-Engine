namespace XuanYu.World.Terrain.Source;

public sealed class TerrainSourceRaster
{
    public TerrainSourceRaster(int width, int height, double[] elevationMeters, bool[] noDataMask)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (elevationMeters.Length != width * height) throw new ArgumentException("高程样本数量与栅格尺寸不一致。", nameof(elevationMeters));
        if (noDataMask.Length != elevationMeters.Length) throw new ArgumentException("NoData 掩码数量与高程样本不一致。", nameof(noDataMask));
        Width = width;
        Height = height;
        ElevationMeters = elevationMeters.ToArray();
        NoDataMask = noDataMask.ToArray();
    }

    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<double> ElevationMeters { get; }
    public IReadOnlyList<bool> NoDataMask { get; }
}
