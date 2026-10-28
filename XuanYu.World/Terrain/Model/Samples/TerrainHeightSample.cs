namespace XuanYu.World.Terrain;

public readonly record struct TerrainHeightSample
{
    private readonly double _meters;

    private TerrainHeightSample(bool isValid, double meters)
    {
        IsValid = isValid;
        _meters = meters;
    }

    public bool IsValid { get; }
    public bool IsNoData => !IsValid;

    public double Meters => IsValid
        ? _meters
        : throw new InvalidOperationException("NoData 没有米制高度值。");

    public static TerrainHeightSample NoData => default;

    public static TerrainHeightSample Valid(double meters) => new(true, meters);
}
