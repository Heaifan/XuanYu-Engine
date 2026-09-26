using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class PolygonVisualCenterTests
{
    [Fact]
    public void Convex_polygon_returns_an_interior_anchor()
    {
        var polygon = new MapPoint[] { new(0, 0), new(10, 0), new(10, 10), new(0, 10) };

        Assert.True(PolygonVisualCenter.TryFind(polygon, out var anchor));
        Assert.InRange(anchor.X, 0.0, 10.0);
        Assert.InRange(anchor.Y, 0.0, 10.0);
    }

    [Fact]
    public void Concave_polygon_anchor_does_not_fall_into_the_notch()
    {
        var polygon = new MapPoint[]
        {
            new(0, 0), new(10, 0), new(10, 10), new(6, 10),
            new(6, 4), new(0, 4)
        };

        Assert.True(PolygonVisualCenter.TryFind(polygon, out var anchor));
        Assert.True(anchor.Y <= 4 || anchor.X >= 6);
    }

    [Fact]
    public void Degenerate_polygon_is_rejected()
    {
        Assert.False(PolygonVisualCenter.TryFind([new(1, 1), new(1, 1)], out _));
    }

    [Fact]
    public void Result_is_deterministic()
    {
        var polygon = new MapPoint[] { new(-3, -1), new(7, -2), new(8, 6), new(-2, 5) };

        Assert.True(PolygonVisualCenter.TryFind(polygon, out var first));
        Assert.True(PolygonVisualCenter.TryFind(polygon, out var second));
        Assert.Equal(first, second);
    }
}
