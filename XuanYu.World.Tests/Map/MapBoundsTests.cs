using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1：有限地图边界合同（中心原点、闭区间、尺寸变化同步）。
public sealed class MapBoundsTests
{
    static readonly MapSize Size10x8 = new(10000.0, 8000.0);

    [Fact]
    public void Center_origin_halves_size()
    {
        Assert.Equal(-5000.0, MapBounds.MinX(Size10x8));
        Assert.Equal(5000.0, MapBounds.MaxX(Size10x8));
        Assert.Equal(-4000.0, MapBounds.MinY(Size10x8));
        Assert.Equal(4000.0, MapBounds.MaxY(Size10x8));
    }

    [Theory]
    [InlineData(-5000.0, 0.0)]
    [InlineData(5000.0, 0.0)]
    [InlineData(0.0, -4000.0)]
    [InlineData(0.0, 4000.0)]
    [InlineData(0.0, 0.0)]
    public void Boundary_belongs_to_map(double x, double y)
    {
        Assert.True(MapBounds.Contains(Size10x8, x, y));
    }

    [Theory]
    [InlineData(5000.1, 0.0)]
    [InlineData(-5000.1, 0.0)]
    [InlineData(0.0, 4000.1)]
    [InlineData(0.0, -4000.1)]
    [InlineData(6000.0, 5000.0)]
    public void Outside_boundary_rejected(double x, double y)
    {
        Assert.False(MapBounds.Contains(Size10x8, x, y));
    }

    [Fact]
    public void Resized_map_updates_bounds()
    {
        var bigger = new MapSize(20000.0, 10000.0);
        Assert.Equal(-10000.0, MapBounds.MinX(bigger));
        Assert.Equal(5000.0, MapBounds.MaxY(bigger));
        Assert.True(MapBounds.Contains(bigger, 9000.0, 4000.0));
    }
}
