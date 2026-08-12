using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetRegionLayerF3Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-layer-f3-{Guid.NewGuid():N}");

    [Fact]
    public async Task Layer_rename_updates_dataset_and_remove_keeps_file()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        var id = vm.DatasetSelectedId!;
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        var layer = vm.LayerItems.Single(item => item.LayerId == MapDatasetLayerIdProjection.Project(id));
        vm.SelectedLayer = layer;

        Assert.True(await vm.CommitLayerRenameAsync("广东"));
        Assert.Equal("广东", vm.SelectedDataset!.Name);
        Assert.Equal("广东", vm.LayerItems.Single(item => item.LayerId == layer.LayerId).Name);

        vm.DangerousCommandConfirmRequested += vm.ConfirmDangerousCommand;
        vm.RunCommand.Execute("删除图层");
        for (var i = 0; i < 500 && vm.RegionDatasetItems.Count != 0; i++) await Task.Delay(10);
        Assert.Empty(vm.RegionDatasetItems);
        Assert.True(File.Exists(Path.Combine(_root, "data", $"{id}.json")));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch (IOException) { }
    }
}
