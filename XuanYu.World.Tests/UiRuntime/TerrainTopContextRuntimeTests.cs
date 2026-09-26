using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TerrainTopContextRuntimeTests
{
    [Fact]
    public void Terrain_context_hides_region_transform_tools_and_restores_them()
    {
        var vm = new UiVm(null, seedInitialScene: false);
        vm.ToggleEditorMode();

        vm.EnterTerrainContext();
        Assert.True(vm.IsTerrainContext);
        Assert.False(vm.IsRegionContext);
        Assert.Equal("地形", vm.ContextButtonLabel);

        vm.EnterRegionContext();
        Assert.False(vm.IsTerrainContext);
        Assert.True(vm.IsRegionContext);
        Assert.Equal("区域", vm.ContextButtonLabel);
    }

    [Fact]
    public void Import_terrain_source_loads_supported_ascii_grid_data()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "ncols 2\nnrows 2\nxllcorner 0\nyllcorner 0\ncellsize 1\nnodata_value -9999\n1 2\n3 -9999\n");
        try
        {
            var vm = new UiVm(null, seedInitialScene: false);
            Assert.True(vm.ImportTerrainSource(path));
            Assert.NotNull(vm.TerrainSource);
            Assert.NotNull(vm.TerrainWorld);
            Assert.Equal(2, vm.TerrainSource!.Raster.Width);
            Assert.Equal("地形源已加载。", vm.TerrainStatus);
        }
        finally { File.Delete(path); }
    }
}
