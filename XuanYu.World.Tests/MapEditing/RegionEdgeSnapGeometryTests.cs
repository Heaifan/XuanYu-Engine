using XuanYu.Core.Gizmo;
using XuanYu.Editor.MapEditing;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionEdgeSnapGeometryTests
{
    [Fact]
    public void Horizontal_segment_returns_perpendicular_closest_point()
    {
        var ok = RegionEdgeSnapGeometry.TryClosestPoint(new(5, 3), new(0, 0), new(10, 0), out var point, out var t);
        Assert.True(ok); Assert.Equal(new ScreenPoint(5, 0), point); Assert.Equal(0.5, t);
    }

    [Fact]
    public void Vertical_segment_returns_perpendicular_closest_point()
    {
        var ok = RegionEdgeSnapGeometry.TryClosestPoint(new(3, 5), new(0, 0), new(0, 10), out var point, out _);
        Assert.True(ok); Assert.Equal(new ScreenPoint(0, 5), point);
    }

    [Fact]
    public void Diagonal_segment_returns_projected_closest_point()
    {
        var ok = RegionEdgeSnapGeometry.TryClosestPoint(new(0, 5), new(0, 0), new(10, 10), out var point, out _);
        Assert.True(ok); Assert.Equal(new ScreenPoint(2.5, 2.5), point);
    }

    [Fact]
    public void Outside_point_clamps_to_segment_endpoint()
    {
        var ok = RegionEdgeSnapGeometry.TryClosestPoint(new(-3, 3), new(0, 0), new(10, 0), out var point, out var t);
        Assert.True(ok); Assert.Equal(new ScreenPoint(0, 0), point); Assert.Equal(0, t);
    }

    [Fact]
    public void Zero_length_segment_is_rejected()
    {
        var ok = RegionEdgeSnapGeometry.TryClosestPoint(new(1, 1), new(2, 2), new(2, 2), out _, out _);
        Assert.False(ok);
    }
}
