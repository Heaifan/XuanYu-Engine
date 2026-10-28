namespace XuanYu.World.Terrain.Source;

public readonly record struct TerrainGeographicBounds
{
    public TerrainGeographicBounds(double west, double south, double east, double north)
    {
        if (!double.IsFinite(west) || !double.IsFinite(south) || !double.IsFinite(east) || !double.IsFinite(north))
            throw new ArgumentOutOfRangeException(nameof(west), "地理范围必须是有限数值。");
        if (east <= west || north <= south) throw new ArgumentException("地理范围必须具有正面积。");
        West = west;
        South = south;
        East = east;
        North = north;
    }

    public double West { get; }
    public double South { get; }
    public double East { get; }
    public double North { get; }
}
