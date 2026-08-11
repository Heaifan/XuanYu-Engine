using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetLayerR3Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-layer-r3-{Guid.NewGuid():N}");

    async Task<(UiVm Vm, string Path, string Road, string Region)> ReadyAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "road"; Assert.True(await vm.CreateDatasetAsync());
        var road = vm.DatasetItems.Single().Id;
        vm.DatasetCreateType = "region"; Assert.True(await vm.CreateDatasetAsync());
        return (vm, path, road, vm.DatasetItems.Single(item => item.Id != road).Id);
    }

    [Fact]
    public async Task Visibility_and_lock_change_state_without_changing_selection()
    {
        var (vm, _, road, region) = await ReadyAsync();
        vm.SelectDataset(road);
        await vm.ToggleDatasetVisibilityAsync(region);
        await vm.ToggleDatasetLockAsync(region);
        Assert.Equal(road, vm.DatasetSelectedId);
        var row = vm.DatasetItems.Single(item => item.Id == region);
        Assert.False(row.IsVisible); Assert.True(row.IsLocked);
        Assert.True(vm.HasUnsavedMapChanges);
    }

    [Fact]
    public async Task Locked_dataset_cannot_unregister_but_hidden_unlocked_dataset_can()
    {
        var (vm, _, road, region) = await ReadyAsync();
        await vm.ToggleDatasetLockAsync(region); vm.SelectDataset(region);
        Assert.False(vm.CanUnregisterDataset);
        Assert.False(await vm.UnregisterDatasetAsync());
        await vm.ToggleDatasetLockAsync(region); await vm.ToggleDatasetVisibilityAsync(region);
        Assert.True(await vm.UnregisterDatasetAsync());
        Assert.DoesNotContain(vm.DatasetItems, item => item.Id == region);
        Assert.Contains(vm.DatasetItems, item => item.Id == road);
    }

    [Fact]
    public async Task Reorder_save_reload_preserves_normalized_layer_state()
    {
        var (vm, path, road, region) = await ReadyAsync();
        await vm.ReorderDatasetLayerAsync(region, 0);
        await vm.ToggleDatasetVisibilityAsync(road);
        await vm.ToggleDatasetLockAsync(region);
        Assert.True(await vm.SaveMapManifestAsync(path));
        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        Assert.Equal([region, road], reopened.DatasetItems.Select(item => item.Id));
        Assert.Equal([0, 1], reopened.DatasetItems.Select(item => item.Order));
        Assert.True(reopened.DatasetItems[0].IsLocked);
        Assert.False(reopened.DatasetItems[1].IsVisible);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
