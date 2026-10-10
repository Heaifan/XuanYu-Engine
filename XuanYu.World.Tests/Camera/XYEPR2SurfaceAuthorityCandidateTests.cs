using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;
using XuanYu.World.Tests.Terrain;

namespace XuanYu.World.Tests.Camera;

/// <summary>
/// Characterization evidence for distinct DEM query and rendered-mesh surfaces.
/// These assertions intentionally record differences; they do not define a parity contract.
/// </summary>
public sealed class XYEPR2SurfaceAuthorityCandidateTests
{
    [Fact]
    public void Center_peak_is_present_in_source_and_full_resolution_mesh_but_absent_at_lod1()
    {
        using var stream = TerrainImportFixture.HgtStream(
            0, 0, 0,
            0, 100, 0,
            0, 0, 0);
        var tile = new HgtTerrainElevationTileReader().Read(stream, "n23e121");
        var world = TerrainWorld.FromElevationTile(tile);
        var field = world.ToHeightfield();
        var chunk = TerrainChunkPartitioner.Partition(field, "peak", 1).Single();

        var sourceCenter = tile.GetElevation(23.5, 121.5);
        var worldCenter = world.QueryElevation(new(1, 1));
        var lod0 = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 2.0);
        var lod1 = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod1, 2.0);
        var lod1CenterOnDiagonal = (lod1.Vertices[1].Z + lod1.Vertices[2].Z) / 2.0;

        Assert.Equal(100, sourceCenter.ElevationMeters);
        Assert.Equal(100, worldCenter.ElevationMeters);
        Assert.Equal(200, lod0.Vertices[4].Z);
        Assert.Equal(0, lod1CenterOnDiagonal);
        // Source/sample values and exaggerated visual mesh heights are separate quantities.
    }

    [Fact]
    public void Saddle_center_exposes_bilinear_versus_render_triangle_interpolation()
    {
        using var stream = TerrainImportFixture.HgtStream(100, 0, 0, 100);
        var tile = new HgtTerrainElevationTileReader().Read(stream, "n23e121");
        var world = TerrainWorld.FromElevationTile(tile);
        var field = world.ToHeightfield();
        var chunk = TerrainChunkPartitioner.Partition(field, "saddle", 1).Single();
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 2.0);

        var sourceBilinearCenter = tile.GetElevation(23.5, 121.5);
        // The mesh diagonal runs between the two 100 m corners; at its midpoint
        // linear triangle interpolation is 100 m, then the visual transform doubles it.
        var exaggeratedTriangleCenter = (mesh.Vertices[1].Z + mesh.Vertices[2].Z) / 2.0;

        Assert.Equal(50, sourceBilinearCenter.ElevationMeters);
        Assert.Equal(200, exaggeratedTriangleCenter);
    }

    [Fact]
    public void Narrow_ridge_is_missing_from_coarse_lod_triangle_mesh()
    {
        using var stream = TerrainImportFixture.HgtStream(
            0, 100, 0,
            0, 100, 0,
            0, 100, 0);
        var world = TerrainWorld.FromElevationTile(
            new HgtTerrainElevationTileReader().Read(stream, "n23e121"));
        var field = world.ToHeightfield();
        var chunk = TerrainChunkPartitioner.Partition(field, "ridge", 1).Single();
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod1, 5);

        Assert.Equal(100, world.QueryElevation(new(1, 1)).ElevationMeters);
        Assert.Equal(0, mesh.Vertices[0].Z);
        Assert.Equal(0, mesh.Vertices[1].Z);
        Assert.Equal(0, mesh.Vertices[2].Z);
        Assert.Equal(0, mesh.Vertices[3].Z);
    }
}
