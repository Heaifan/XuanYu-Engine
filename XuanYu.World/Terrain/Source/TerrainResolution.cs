namespace XuanYu.World.Terrain.Source;

public readonly record struct TerrainResolution
{
    public TerrainResolution(double x, double y)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y) || x <= 0 || y <= 0)
            throw new ArgumentOutOfRangeException(nameof(x), "栅格分辨率必须是正的有限数值。");
        X = x;
        Y = y;
    }

    public double X { get; }
    public double Y { get; }
}
