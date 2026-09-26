using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TerrainRenderIntegrationTests
{
    [Fact]
    public void Default_vertical_exaggeration_is_one()
    {
        Assert.Equal(1.0, TerrainRenderTransform.Default.VerticalExaggeration);
    }

    [Fact]
    public void Imported_terrain_is_present_in_ui_render_projection()
    {
        var vm = NewVm();
        var path = WriteFixture();
        try
        {
            Assert.True(vm.ImportTerrainSource(path));
            Assert.True(vm.RenderProjection.Success);
            Assert.True(vm.RenderProjection.Projection.HasTerrain);
            Assert.Equal(2, vm.RenderProjection.Projection.Terrain!.Heightfield.Width);
            Assert.Equal(1.0, vm.RenderProjection.Projection.EffectiveTerrainTransform.VerticalExaggeration);
            vm.VerticalExaggeration = 5.0;
            Assert.Equal(5.0, vm.RenderProjection.Projection.EffectiveTerrainTransform.VerticalExaggeration);
            Assert.Equal(100.0, vm.TerrainWorld!.QueryHeight(new(0, 0)));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Vertical_exaggeration_change_invalidates_render_projection()
    {
        var vm = NewVm();
        var changes = 0;
        vm.RenderProjectionChanged += _ => changes++;

        vm.VerticalExaggeration = 5.0;

        Assert.True(changes > 0);
    }

    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);

    static string WriteFixture()
    {
        var path = Path.Combine(Path.GetTempPath(), $"cut1-{Guid.NewGuid():N}.asc");
        File.WriteAllText(path, "ncols 2\nnrows 2\nxllcorner 0\nyllcorner 0\ncellsize 1\nNODATA_value -9999\n100 100\n100 100\n");
        return path;
    }
}
