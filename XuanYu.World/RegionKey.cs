namespace XuanYu.World;

public readonly record struct RegionKey
{
    public RegionKey(int x, int y, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public static RegionKey Origin { get; } = new(0, 0, 0);

    public static RegionKey FromGrid(int x, int y, int z = 0) => new(x, y, z);

    public override string ToString() => $"Region({X},{Y},{Z})";
}
