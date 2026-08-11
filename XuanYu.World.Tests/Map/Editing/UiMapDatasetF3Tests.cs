using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetF3Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-ui-f3-{Guid.NewGuid():N}");

    async Task<(UiVm Vm, string Path)> SavedVmAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        return (vm, path);
    }

    async Task<(UiVm Vm, string First, string Second)> TwoDatasetsAsync()
    {
        var (vm, _) = await SavedVmAsync();
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        var first = vm.DatasetItems[0].Id;
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        return (vm, first, vm.DatasetItems[1].Id);
    }

    [Fact]
    public async Task F3_selection_is_single_and_drives_layer_projection()
    {
        var (vm, first, second) = await TwoDatasetsAsync();
        Assert.Equal(second, vm.DatasetSelectedId);
        Assert.Equal(second, vm.SelectedDataset!.Id);
        Assert.Equal(vm.DatasetItems, vm.DatasetLayerItems);
        vm.SelectDataset(first);
        Assert.Equal(first, vm.DatasetSelectedId);
        Assert.Single(vm.DatasetItems, item => item.IsSelected);
        Assert.Equal(first, vm.DatasetLayerItems.Single(item => item.IsSelected).Id);
    }

    [Fact]
    public async Task F3_unregister_uses_selection_and_migrates_to_next_available()
    {
        var (vm, first, second) = await TwoDatasetsAsync();
        Assert.True(await vm.UnregisterDatasetAsync());
        Assert.DoesNotContain(vm.DatasetItems, item => item.Id == second);
        Assert.Equal(first, vm.DatasetSelectedId);
        Assert.True(File.Exists(Path.Combine(_root, "data", $"{second}.json")));
    }

    [Fact]
    public async Task F3_no_selection_fails_closed_and_is_not_unregisterable()
    {
        var (vm, _) = await SavedVmAsync();
        Assert.False(vm.CanUnregisterDataset);
        Assert.False(await vm.UnregisterDatasetAsync());
        Assert.Contains("先选择", vm.FooterMessage);
        Assert.Empty(vm.DatasetItems);
    }

    [Fact]
    public async Task F3_reload_rebuilds_layer_projection_from_registry()
    {
        var (vm, _, _) = await TwoDatasetsAsync();
        var path = Path.Combine(_root, "map.json");
        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        Assert.Equal(2, reopened.DatasetLayerItems.Count);
        Assert.Null(reopened.DatasetSelectedId);
        reopened.SelectDataset(vm.DatasetItems[0].Id);
        Assert.Equal(vm.DatasetItems[0].Id, reopened.SelectedDataset!.Id);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
