using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R2-D3：有限地面常量几何合同——固定 4 顶点 6 索引，尺寸只进顶点坐标。
public sealed class MapSurfaceGeometryTests
{
    static MapRenderSnapshot Snapshot(double width, double depth, double baseHeight = 0) => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", width, depth,
        MapSurfaceKind.Flat, baseHeight, 0, 1, 1, 1);

    [Fact]
    public void Default_map_uses_constant_geometry()
    {
        var g = MapSurfaceGeometryBuilder.Build(Snapshot(10000, 10000));

        Assert.Equal(4, g.Vertices.Length);
        Assert.Equal(6, g.Indices.Length);
    }

    [Fact]
    public void Twenty_km_map_still_four_vertices()
    {
        var g = MapSurfaceGeometryBuilder.Build(Snapshot(20000, 20000));

        Assert.Equal(4, g.Vertices.Length);
        Assert.Equal(6, g.Indices.Length);
    }

    [Fact]
    public void Max_size_geometry_remains_constant()
    {
        var g = MapSurfaceGeometryBuilder.Build(Snapshot(1000000, 1000000));

        Assert.Equal(4, g.Vertices.Length);
    }

    [Fact]
    public void Corners_are_symmetric_around_origin()
    {
        var g = MapSurfaceGeometryBuilder.Build(Snapshot(2000, 4000));

        Assert.Equal(-1000, g.Vertices[0].X);
        Assert.Equal(-2000, g.Vertices[0].Y);
        Assert.Equal(1000, g.Vertices[1].X);
        Assert.Equal(-2000, g.Vertices[1].Y);
        Assert.Equal(1000, g.Vertices[2].X);
        Assert.Equal(2000, g.Vertices[2].Y);
        Assert.Equal(-1000, g.Vertices[3].X);
        Assert.Equal(2000, g.Vertices[3].Y);
    }

    [Fact]
    public void Base_height_enters_z_coordinate()
    {
        var g = MapSurfaceGeometryBuilder.Build(Snapshot(2000, 2000, 100));

        Assert.All(g.Vertices, v => Assert.Equal(100f, v.Z));
        Assert.All(g.Vertices, v => Assert.Equal(0f, v.Nx));
        Assert.All(g.Vertices, v => Assert.Equal(0f, v.Ny));
        Assert.All(g.Vertices, v => Assert.Equal(1f, v.Nz));
        Assert.All(g.Vertices, v => Assert.Equal(1f, v.Brightness));
    }

    [Fact]
    public void Indices_form_two_triangles()
    {
        var g = MapSurfaceGeometryBuilder.Build(Snapshot(2000, 2000));

        Assert.Equal([0u, 1u, 2u, 0u, 2u, 3u], g.Indices);
    }

    [Fact]
    public void Bounds_geometry_is_four_strips_only()
    {
        var bounds = MapBoundsGeometryBuilder.Build(Snapshot(2000, 2000));

        Assert.Equal(24, bounds.Length);
    }

    [Fact]
    public void Bounds_strip_width_scales_with_size()
    {
        Assert.Equal(10.0, MapBoundsGeometryBuilder.StripWidthMeters(Snapshot(10000, 10000)));
        Assert.Equal(20.0, MapBoundsGeometryBuilder.StripWidthMeters(Snapshot(20000, 20000)));
        Assert.Equal(1.0, MapBoundsGeometryBuilder.StripWidthMeters(Snapshot(200, 200)));
        Assert.Equal(50.0, MapBoundsGeometryBuilder.StripWidthMeters(Snapshot(1000000, 1000000)));
    }

    [Fact]
    public void Bounds_sits_at_base_height_plus_render_lift()
    {
        var bounds = MapBoundsGeometryBuilder.Build(Snapshot(2000, 2000, 50));

        Assert.All(bounds, v => Assert.Equal(50.05f, v.Z));
    }
}
