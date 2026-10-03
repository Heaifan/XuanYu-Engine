using XuanYu.World;
using XuanYu.World.Terrain;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainWorldAuthorityTests
{
    [Fact]
    public void Terrain_world_exposes_typed_query_and_owned_revision()
    {
        var edit = TerrainHeightLayer.CreateEditDelta();
        var world = new TerrainWorld(
            TerrainHeightLayer.CreateBase((new TerrainSampleCoordinate(0, 0), 10)), edit);

        var before = world.TerrainRevision.Value;
        edit.Set(new(0, 0), 2);
        var result = world.QueryElevation(new(0, 0));

        Assert.Equal(WorldQueryStatus.Valid, result.Status);
        Assert.Equal(12, result.ElevationMeters);
        Assert.True(world.TerrainRevision.Value > before);
    }

    [Fact]
    public void Terrain_world_keeps_render_metadata_out_of_query_storage()
    {
        var world = new TerrainWorld(
            TerrainHeightLayer.CreateBase((new TerrainSampleCoordinate(0, 0), 10)),
            renderRowsSouthToNorth: true);

        Assert.True(world.RasterOrientation.RowsSouthToNorth);
        Assert.Equal(world.RasterOrientation.RowsSouthToNorth,
            world.RenderRowsSouthToNorth);
    }
}
