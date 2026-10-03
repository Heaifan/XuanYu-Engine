using XuanYu.Core.Math;
using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class MapSurfacePatchGeometryTests
{
    static MapRenderSnapshot Snapshot() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", 10000, 10000,
        MapSurfaceKind.Flat, 120, 0, 1, 1, 1);

    [Fact]
    public void Patch_uses_footprint_bounds_in_render_space()
    {
        var patch = new ReferencePlanePatchPlacement(100, 200, 400, 600);
        var geometry = MapSurfaceGeometryBuilder.BuildPatch(
            Snapshot(), patch, new Vector3d(80, 150, 20));

        Assert.Equal(100, geometry.Vertices[0].Z, 5);
        Assert.Equal(-180, geometry.Vertices[0].X, 5);
        Assert.Equal(220, geometry.Vertices[2].X, 5);
        Assert.Equal(-250, geometry.Vertices[0].Y, 5);
        Assert.Equal(350, geometry.Vertices[2].Y, 5);
    }
}
