using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionSpatialBounds
{
    public RegionSpatialBounds(double minX, double minY, double maxX, double maxY)
    {
        if (!double.IsFinite(minX) || !double.IsFinite(minY) ||
            !double.IsFinite(maxX) || !double.IsFinite(maxY))
            throw new ArgumentOutOfRangeException(nameof(maxX), "区域查询边界必须为有限数值。");
        if (minX > maxX || minY > maxY)
            throw new ArgumentOutOfRangeException(nameof(maxX), "区域查询最大边界不得小于最小边界。");
        MinX = minX; MinY = minY; MaxX = maxX; MaxY = maxY;
    }

    public double MinX { get; }
    public double MinY { get; }
    public double MaxX { get; }
    public double MaxY { get; }
    internal double Perimeter => 2.0 * ((MaxX - MinX) + (MaxY - MinY));
    public static RegionSpatialBounds From(MapRegion region)
    {
        var first = region.Vertices[0];
        var minX = first.X; var minY = first.Y; var maxX = first.X; var maxY = first.Y;
        foreach (var point in region.Vertices)
        {
            minX = Math.Min(minX, point.X); minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X); maxY = Math.Max(maxY, point.Y);
        }
        return new(minX, minY, maxX, maxY);
    }

    public bool Contains(RegionSpatialBounds other) =>
        MinX <= other.MinX && MinY <= other.MinY && MaxX >= other.MaxX && MaxY >= other.MaxY;

    public bool Intersects(RegionSpatialBounds other) =>
        MinX <= other.MaxX && MaxX >= other.MinX && MinY <= other.MaxY && MaxY >= other.MinY;

    internal RegionSpatialBounds Union(RegionSpatialBounds other) => new(
        Math.Min(MinX, other.MinX), Math.Min(MinY, other.MinY),
        Math.Max(MaxX, other.MaxX), Math.Max(MaxY, other.MaxY));
}
