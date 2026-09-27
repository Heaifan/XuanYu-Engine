using System.Buffers.Binary;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainImportFix1BUiContractTests
{
    [Fact]
    public void Import_panel_keeps_progress_and_cancel_without_completed_card()
    {
        var source = TerrainFix1BSource.Read("XuanYu.Editor.UI/Win/UiWin.axaml");

        Assert.Contains("IsVisible=\"{Binding IsTerrainImporting}\"", source);
        Assert.Contains("xy:XYProgressBar", source);
        Assert.Contains("CancelTerrainImportCommand", source);
        Assert.DoesNotContain("IsTerrainImportComplete", source);
        Assert.DoesNotContain("导入完成</xy:XYHeading>", source);
    }

    [Fact]
    public async Task Completed_hgt_import_uses_footer_and_existing_terrain_inspector()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.DatasetCreateType = MapDatasetTypes.TerrainArea;
        Assert.True(await vm.CreateDatasetAsync());
        var directory = Path.Combine(Path.GetTempPath(), $"xye-terrain-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "n23e121.hgt");
        try
        {
            WriteHgt(path, 100, 200, 300, 400);
            Assert.True(await vm.ImportTerrainSourceAsync(path));

            Assert.False(vm.IsTerrainImporting);
            Assert.True(vm.IsTerrainImportComplete);
            Assert.Equal("导入完成：n23e121 · 2 × 2", vm.FooterMessage);
            Assert.True(vm.IsTerrainInspector);
            Assert.Contains(vm.InspectorProperties, item => item.DisplayName == "网格" && item.Value == "2 × 2");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Empty_terrain_state_is_safe_for_inspector_reads()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        Assert.Null(vm.TerrainInspectorMetadata);
        Assert.False(vm.IsTerrainInspector);
        Assert.NotNull(vm.InspectorProperties);
        Assert.NotNull(vm.InspectorFields);
    }

    static void WriteHgt(string path, params short[] values)
    {
        using var stream = File.Create(path);
        Span<byte> sample = stackalloc byte[2];
        foreach (var value in values)
        {
            BinaryPrimitives.WriteInt16BigEndian(sample, value);
            stream.Write(sample);
        }
    }
}

static class TerrainFix1BSource
{
    public static string RootPath(string relative) =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../", relative));

    public static string Read(string relative) => File.ReadAllText(RootPath(relative));
}
