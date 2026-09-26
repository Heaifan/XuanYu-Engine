using XuanYu.World.Terrain;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainWorldTests
{
    [Fact]
    public void Final_height_is_base_plus_edit_delta()
    {
        var world = CreateWorld(12.5, 2.25);

        Assert.Equal(14.75, world.GetFinalHeight(Coordinate).Meters);
    }

    [Fact]
    public void Missing_edit_delta_is_zero_not_nodata()
    {
        var world = CreateWorld(-3.5);

        Assert.Equal(-3.5, world.GetFinalHeight(Coordinate).Meters);
        Assert.True(world.EditDelta.Read(Coordinate).IsValid);
        Assert.Equal(0.0, world.EditDelta.Read(Coordinate).Meters);
    }

    [Fact]
    public void Positive_zero_and_negative_heights_are_meter_values()
    {
        var world = new TerrainWorld(TerrainHeightLayer.CreateBase(
            (Coordinate, 8.0), (new(1, 0), 0.0), (new(2, 0), -4.0)));

        Assert.Equal(8.0, world.GetFinalHeight(Coordinate).Meters);
        Assert.Equal(0.0, world.GetFinalHeight(new(1, 0)).Meters);
        Assert.Equal(-4.0, world.GetFinalHeight(new(2, 0)).Meters);
    }

    [Fact]
    public void Base_nodata_remains_final_nodata()
    {
        var world = new TerrainWorld(TerrainHeightLayer.CreateBase());

        Assert.True(world.GetFinalHeight(Coordinate).IsNoData);
        Assert.False(world.TryGetFinalHeight(Coordinate, out _));
    }

    [Fact]
    public void Editing_delta_does_not_mutate_base()
    {
        var world = CreateWorld(10.0);

        world.EditDelta.Set(Coordinate, -2.0);

        Assert.Equal(10.0, world.BaseHeight.Read(Coordinate).Meters);
        Assert.Equal(8.0, world.GetFinalHeight(Coordinate).Meters);
    }

    [Fact]
    public void Base_layer_cannot_be_used_as_edit_delta()
    {
        var baseLayer = TerrainHeightLayer.CreateBase();

        Assert.Throws<ArgumentException>(() => new TerrainWorld(baseLayer, baseLayer));
    }

    [Fact]
    public void Tile_lod_is_derived_metadata_without_height_storage()
    {
        var descriptor = new TerrainTileLod(new(4, 8), 16, 2);

        Assert.Equal(new(4, 8), descriptor.Origin);
        Assert.Equal(16, descriptor.SampleSize);
        Assert.Equal(2, descriptor.Level);
        Assert.DoesNotContain(typeof(TerrainTileLod).GetProperties(),
            property => property.PropertyType == typeof(TerrainHeightLayer));
    }

    static readonly TerrainSampleCoordinate Coordinate = new(0, 0);

    static TerrainWorld CreateWorld(double baseHeight, double delta = 0.0)
    {
        var edit = TerrainHeightLayer.CreateEditDelta();
        edit.Set(Coordinate, delta);
        return new(TerrainHeightLayer.CreateBase((Coordinate, baseHeight)), edit);
    }
}
