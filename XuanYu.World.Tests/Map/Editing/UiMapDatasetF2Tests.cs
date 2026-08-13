using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetF2Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-ui-f2-{Guid.NewGuid():N}");

    async Task<UiVm> SavedVmAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        return vm;
    }

    [Fact]
    public async Task F2_create_refreshes_rows_and_hides_empty_state()
    {
        var vm = await SavedVmAsync();
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        Assert.False(vm.IsDatasetEmpty);
        Assert.Contains("1", vm.DatasetEmptyState);
        Assert.Equal("road", vm.DatasetItems[0].Type);
        Assert.Equal("道路", vm.DatasetItems[0].TypeDisplay);
        Assert.Matches("^road-[0-9a-f]{6}$", vm.DatasetItems[0].Id);
    }

    [Fact]
    public void F2_type_presentation_maps_all_types_without_changing_domain_values()
    {
        var expected = new[] { "区域面", "道路", "地图标记", "城镇", "资源", "河流", "地形区域" };
        Assert.Equal(expected, MapDatasetTypePresentation.Options.Select(item => item.Display));
        Assert.Equal("road", MapDatasetTypePresentation.Options[1].Value);
    }

    [Fact]
    public async Task F2_failed_create_keeps_list_empty()
    {
        var vm = await SavedVmAsync();
        await File.WriteAllTextAsync(Path.Combine(_root, "data"), "not-a-directory");
        vm.DatasetCreateType = "road";
        Assert.False(await vm.CreateDatasetAsync());
        Assert.Empty(vm.DatasetItems);
        Assert.True(vm.IsDatasetEmpty);
    }

    [Fact]
    public async Task F2_reopen_preserves_multiple_generated_datasets()
    {
        var vm = await SavedVmAsync();
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        Assert.True(await vm.CreateDatasetAsync());
        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(Path.Combine(_root, "map.json")));
        Assert.Equal(2, reopened.DatasetItems.Count);
        Assert.All(reopened.DatasetItems, item => Assert.Equal("road", item.Type));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
