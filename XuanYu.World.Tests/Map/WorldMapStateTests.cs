using XuanYu.Core.Map;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D3：World 地图状态——有限边界（闭区间）与高度查询。
public sealed class WorldMapStateTests
{
    static WorldMapState State() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", "TestBattlefield",
        2000.0, 2000.0, MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1);

    [Fact]
    public void Center_is_inside()
    {
        Assert.True(State().Contains(0, 0));
    }

    [Fact]
    public void Boundary_edges_are_inside_closed_interval()
    {
        var s = State();
        Assert.True(s.Contains(-1000, 0));
        Assert.True(s.Contains(1000, 0));
        Assert.True(s.Contains(0, -1000));
        Assert.True(s.Contains(0, 1000));
        Assert.True(s.Contains(1000, 1000));
    }

    [Theory]
    [InlineData(1000.001, 0)]
    [InlineData(-1000.001, 0)]
    [InlineData(0, 1000.001)]
    [InlineData(0, -1000.001)]
    [InlineData(2000, 2000)]
    public void Outside_is_rejected(double x, double y)
    {
        Assert.False(State().Contains(x, y));
    }

    [Fact]
    public void Query_inside_returns_height()
    {
        var s = State();
        Assert.True(s.TryGetSurfaceHeight(0, 0, out var z));
        Assert.Equal(s.SampleHeight(0, 0), z);
    }

    [Fact]
    public void Query_outside_fails_without_clamping()
    {
        var s = State();
        Assert.False(s.TryGetSurfaceHeight(5000, 0, out var z));
        Assert.Equal(0.0, z);
    }

    [Fact]
    public void Query_on_boundary_succeeds()
    {
        var s = State();
        Assert.True(s.TryGetSurfaceHeight(1000, 0, out var z));
        Assert.Equal(s.SampleHeight(1000, 0), z);
    }

    [Fact]
    public void Query_does_not_echo_clamped_zero()
    {
        var s = State();
        // 地图外 5000 米处高度不等于地图内 5000 会被钳制到的任何值：外部必须失败。
        Assert.False(s.TryGetSurfaceHeight(5000, 5000, out _));
    }

    [Fact]
    public void Non_square_map_bounds_are_respected()
    {
        var s = State() with { WidthMeters = 4000.0, DepthMeters = 1000.0 };
        Assert.True(s.Contains(2000, 0));
        Assert.False(s.Contains(2000.001, 0));
        Assert.True(s.Contains(0, 500));
        Assert.False(s.Contains(0, 500.001));
    }
}
