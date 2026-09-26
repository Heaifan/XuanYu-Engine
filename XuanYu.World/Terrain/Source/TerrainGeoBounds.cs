namespace XuanYu.World.Terrain.Source;

public readonly record struct TerrainGeoBounds
{
    public TerrainGeoBounds(double south, double west, double north, double east)
    {
        if (!double.IsFinite(south) || !double.IsFinite(west) ||
            !double.IsFinite(north) || !double.IsFinite(east) ||
            north <= south || east <= west)
            throw new ArgumentException("地理范围必须具有正面积。", nameof(South));
        South = south; West = west; North = north; East = east;
    }

    public double South { get; }
    public double West { get; }
    public double North { get; }
    public double East { get; }
}
