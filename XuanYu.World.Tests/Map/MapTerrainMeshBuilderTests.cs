using XuanYu.Core.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D4：CPU 地形网格构建器——顶点/索引/高度一致性/法线/边界线亮度。
// 亮度合成测试见 MapTerrainBrightnessTests（F4 拆分，单一职责）。
public sealed class MapTerrainMeshBuilderTests
{
    static MapRenderSnapshot Flat() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", "FlatMap", 2000, 2000,
        MapSurfaceKind.Flat, 5.0, 0.0, 1.0, 1,
        -0.35, -0.55, 0.75, 1.0, 0.35);

    static MapRenderSnapshot Hills() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", "Hills", 2000, 2000,
        MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1,
        -0.35, -0.55, 0.75, 1.0, 0.35);

    [Fact]
    public void Build_produces_expected_vertex_and_index_counts()
    {
        var mesh = MapTerrainMeshBuilder.Build(Hills(), segments: 4);
        Assert.Equal((4 + 1) * (4 + 1), mesh.Vertices.Length);
        Assert.Equal(4 * 4 * 6, mesh.Indices.Length);
    }

    [Fact]
    public void Vertex_world_position_matches_sampler_height()
    {
        var map = Hills();
        var mesh = MapTerrainMeshBuilder.Build(map, segments: 4);
        var expected = (float)MapSurfaceSampler.SampleHeight(
            map.SurfaceKind, map.BaseHeightMeters, map.AmplitudeMeters,
            map.WavelengthMeters, map.Seed, -1000.0, -1000.0);
        Assert.Equal(expected, mesh.Vertices[0].Z, 3);
    }

    [Fact]
    public void Flat_terrain_has_upward_normals()
    {
        var mesh = MapTerrainMeshBuilder.Build(Flat(), segments: 4);
        foreach (var v in mesh.Vertices)
        {
            Assert.Equal(0.0, v.Nx, 6);
            Assert.Equal(0.0, v.Ny, 6);
            Assert.Equal(1.0, v.Nz, 6);
        }
    }

    [Fact]
    public void Hills_terrain_has_varying_height()
    {
        var mesh = MapTerrainMeshBuilder.Build(Hills(), segments: 8);
        var z0 = mesh.Vertices[0].Z;
        Assert.True(mesh.Vertices.Any(v => Math.Abs(v.Z - z0) > 0.01f),
            "缓丘地形应在不同位置产生不同高度");
    }

    [Fact]
    public void Hills_terrain_height_stays_in_range()
    {
        var map = Hills();
        var mesh = MapTerrainMeshBuilder.Build(map, segments: 16);
        foreach (var v in mesh.Vertices)
            Assert.InRange(v.Z, -12.0f - 0.5f, 12.0f + 0.5f);
    }

    [Fact]
    public void Build_is_deterministic()
    {
        var a = MapTerrainMeshBuilder.Build(Hills(), segments: 8);
        var b = MapTerrainMeshBuilder.Build(Hills(), segments: 8);
        Assert.Equal(a.Vertices, b.Vertices);
        Assert.Equal(a.Indices, b.Indices);
    }

    [Fact]
    public void Bounds_builds_48_vertices_with_brightness()
    {
        var bounds = MapBoundsMeshBuilder.BuildBounds(Hills());
        Assert.Equal(48, bounds.Length);
        foreach (var v in bounds)
            Assert.Equal(1.0f, v.Brightness);
    }
}
