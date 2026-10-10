using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Geo;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Tests.Terrain;

namespace XuanYu.World.Tests.Camera;

public sealed class XYEPR2SurfaceAuthorityQueryTests
{
    [Fact]
    public void Hgt_bilinear_ground_pick_and_five_x_render_triangle_report_distinct_heights()
    {
        using var stream = TerrainImportFixture.HgtStream(0, 10, 20, 30);
        var tile = new HgtTerrainElevationTileReader().Read(stream, "n23e121");
        var world = TerrainWorld.FromElevationTile(tile);
        var point = new GeographicWorldMapping(new(23, 121, 0))
            .ToWorld(new(23.75, 121.25, 0));
        var source = tile.GetElevation(23.75, 121.25);
        var ground = GroundPickResolver.Resolve(
            new(new(point.X, point.Y, 100), new(0, 0, -1)),
            new TerrainWorldGroundSurface(world), 0);
        var field = world.ToHeightfield();
        var chunk = TerrainChunkPartitioner.Partition(field, "slope", 1).Single();
        var mesh = TerrainChunkMeshBuilder.Build(field, chunk, TerrainLodLevel.Lod0, 5);
        var rendered = mesh.Vertices[0].Z * 0.5625
            + mesh.Vertices[1].Z * 0.1875
            + mesh.Vertices[2].Z * 0.1875
            + mesh.Vertices[3].Z * 0.0625;

        Assert.Equal(7.5, source.ElevationMeters);
        Assert.True(ground.IsValid);
        Assert.Equal(0, ground.ResolvedElevation);
        Assert.Equal(world.EditDelta.Revision, ground.TerrainRevision);
        Assert.Equal(87.5, rendered);
        Assert.Equal(30, world.QueryElevation(new(1, 1)).ElevationMeters);
    }

    [Fact]
    public void Ground_query_preserves_nodata_and_out_of_bounds_statuses()
    {
        using var stream = TerrainImportFixture.HgtStream(short.MinValue, 10, 20, 30);
        var world = TerrainWorld.FromElevationTile(
            new HgtTerrainElevationTileReader().Read(stream, "n23e121"));
        var mapping = new GeographicWorldMapping(new(23, 121, 0));
        var surface = new TerrainWorldGroundSurface(world);
        var noData = mapping.ToWorld(new(24, 121, 0));
        var edge = mapping.ToWorld(new(24, 122, 0));
        var outside = mapping.ToWorld(new(24.1, 121.5, 0));

        Assert.Equal(WorldQueryStatus.NoData,
            surface.QuerySurface(new(noData.X, noData.Y)).Status);
        Assert.Equal(WorldQueryStatus.OutOfBounds,
            surface.QuerySurface(new(outside.X, outside.Y)).Status);
        Assert.Equal(10, surface.QuerySurface(new(edge.X, edge.Y)).SurfaceZ);
        Assert.Equal(WorldQueryStatus.Valid,
            surface.QuerySurface(new(0, 0)).Status);
    }
}
