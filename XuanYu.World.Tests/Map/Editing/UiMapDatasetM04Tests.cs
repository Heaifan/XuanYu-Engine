using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetM04Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-m04-{Guid.NewGuid():N}");

    async Task<(UiVm Vm, string Road, string Region)> ReadyAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = "road"; Assert.True(await vm.CreateDatasetAsync());
        var road = vm.DatasetItems.Single().Id;
        vm.DatasetCreateType = "region"; Assert.True(await vm.CreateDatasetAsync());
        return (vm, road, vm.DatasetItems.Single(item => item.Id != road).Id);
    }

    [Fact]
    public async Task Invalid_drop_keeps_projection_selection_and_state()
    {
        var (vm, road, region) = await ReadyAsync();
        vm.SelectDataset(region);
        await vm.ToggleDatasetVisibilityAsync(region);
        await vm.ToggleDatasetLockAsync(region);
        var before = vm.DatasetItems.Select(item => item.Id).ToArray();

        await vm.ReorderDatasetLayerAsync(region, -1);

        Assert.Equal(before, vm.DatasetItems.Select(item => item.Id));
        Assert.Equal(region, vm.DatasetSelectedId);
        var row = vm.DatasetItems.Single(item => item.Id == region);
        Assert.False(row.IsVisible);
        Assert.True(row.IsLocked);
        Assert.Equal(2, vm.DatasetLayerItems.Count);
        Assert.Contains(vm.DatasetLayerItems, item => item.Id == road);
    }

    [Fact]
    public async Task Repeated_moves_keep_ids_selection_and_state()
    {
        var (vm, road, region) = await ReadyAsync();
        vm.SelectDataset(region);
        await vm.ToggleDatasetVisibilityAsync(region);
        await vm.ToggleDatasetLockAsync(region);

        await vm.ReorderDatasetLayerAsync(region, 0);
        await vm.ReorderDatasetLayerAsync(road, 1);
        await vm.ReorderDatasetLayerAsync(region, 0);

        Assert.Equal(2, vm.DatasetItems.Count);
        Assert.Equal(2, vm.DatasetItems.Select(item => item.Id).Distinct().Count());
        Assert.Equal(region, vm.DatasetSelectedId);
        var row = vm.DatasetItems.Single(item => item.Id == region);
        Assert.False(row.IsVisible);
        Assert.True(row.IsLocked);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
