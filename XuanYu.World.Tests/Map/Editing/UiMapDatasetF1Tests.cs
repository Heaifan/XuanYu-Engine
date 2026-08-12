using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetF1Tests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-ui-f1-{Guid.NewGuid():N}");

    async Task<(UiVm Vm, string Path)> SavedVmAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = System.IO.Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "road";
        return (vm, path);
    }

    [Fact]
    public async Task F1_A01_to_A06_create_file_manifest_registry_and_normal_ui()
    {
        var (vm, path) = await SavedVmAsync();
        Assert.True(await vm.CreateDatasetAsync());
        var id = vm.DatasetItems[0].Id;
        Assert.Matches("^road-[0-9a-f]{6}$", id);
        var datasetPath = System.IO.Path.Combine(_root, "data", $"{id}.json");
        Assert.True(File.Exists(datasetPath));
        var manifest = await new MapManifestStorageService().LoadAsync(path);
        Assert.Single(manifest.Value!.Datasets);
        Assert.Equal($"data/{id}.json", manifest.Value.Datasets[0].Source);
        var registry = new MapDatasetRegistry(path, manifest.Value);
        Assert.Equal(MapDatasetStatus.Normal, (await registry.FindByIdAsync(id))!.Status);
        Assert.Single(vm.DatasetItems);
        Assert.Equal("正常", vm.DatasetItems[0].Status);
    }

    [Fact]
    public async Task F1_A05_command_route_completes_the_same_create_chain()
    {
        var (vm, _) = await SavedVmAsync();
        vm.RunCommand.Execute("新建数据集");
        for (var i = 0; i < 500 && vm.DatasetItems.Count == 0; i++) await Task.Delay(10);
        Assert.Single(vm.DatasetItems);
        Assert.Matches("^road-[0-9a-f]{6}$", vm.DatasetItems[0].Id);
        Assert.Contains("创建成功", vm.FooterMessage);
    }

    [Fact]
    public async Task F4_A01_to_A05_unsaved_map_creates_dataset_in_working_storage()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        var item = Assert.Single(vm.DatasetItems);
        Assert.Equal("", vm.CurrentMapManifestPath);
        Assert.Equal($"data/{item.Id}.json", vm.CurrentMapManifest.Datasets[0].Source);
        Assert.DoesNotContain(Path.GetTempPath(), item.Source, StringComparison.OrdinalIgnoreCase);
        var path = Path.Combine(_root, "formal", "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        Assert.Equal(path, vm.CurrentMapManifestPath);
        Assert.True(File.Exists(Path.Combine(_root, "formal", "data", $"{item.Id}.json")));
    }

    [Fact]
    public async Task F1_A10_A11_generated_ids_allow_same_type_to_repeat()
    {
        var (vm, _) = await SavedVmAsync();
        Assert.True(await vm.CreateDatasetAsync());
        Assert.True(await vm.CreateDatasetAsync());
        Assert.Equal(2, vm.DatasetItems.Count);
        Assert.NotEqual(vm.DatasetItems[0].Id, vm.DatasetItems[1].Id);
    }

    [Fact]
    public async Task F1_A12_reopen_restores_both_datasets()
    {
        var (vm, path) = await SavedVmAsync();
        Assert.True(await vm.CreateDatasetAsync());
        Assert.True(await vm.CreateDatasetAsync());
        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        Assert.Equal(2, reopened.DatasetItems.Count);
        Assert.All(reopened.DatasetItems, item => Assert.Equal("正常", item.Status));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
