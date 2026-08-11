using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetContractTests : IDisposable
{
    static readonly string DatasetPanel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "DatasetPanel.axaml"));
    static readonly string LayerDock = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "EditorLayerDock.axaml"));
    static readonly string DatasetLayerPanel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "DatasetLayerPanel.axaml"));
    static readonly string EditorPanel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));
    readonly string _directory = Path.Combine(Path.GetTempPath(), $"xuanyu-ui-dataset-{Guid.NewGuid():N}");

    [Fact]
    public void Dataset_page_has_empty_state_create_list_and_unregister_contract()
    {
        Assert.Contains("DatasetEmptyState", DatasetPanel);
        Assert.Contains("新建数据集", DatasetPanel);
        Assert.Contains("DatasetItems", DatasetPanel);
        Assert.Contains("{Binding Name}", DatasetPanel);
        Assert.Contains("{Binding TypeIdText}", DatasetPanel);
        Assert.Contains("{Binding Status}", DatasetPanel);
        Assert.Contains("{Binding Display}", DatasetPanel);
        Assert.DoesNotContain("DatasetCreateId", DatasetPanel);
        Assert.DoesNotContain("Dataset ID", DatasetPanel);
        Assert.Contains("CanUnregisterDataset", DatasetPanel);
        Assert.Contains("解除注册数据集", DatasetPanel);
        Assert.Contains("local:DatasetPanel", EditorPanel);
        Assert.Contains("DatasetLayerPanel", LayerDock);
        Assert.Contains("DatasetLayerItems", DatasetLayerPanel);
        Assert.Contains("DatasetRow_Pressed", DatasetLayerPanel);
        Assert.Contains("DragHandleIcon", DatasetLayerPanel);
        Assert.Contains("VisibleIcon", DatasetLayerPanel);
        Assert.Contains("LockedIcon", DatasetLayerPanel);
        Assert.DoesNotContain("Text=\"拖动\"", DatasetLayerPanel);
        Assert.DoesNotContain("VisibilityActionText", DatasetLayerPanel);
        Assert.DoesNotContain("LockActionText", DatasetLayerPanel);
        Assert.Contains("危险操作", DatasetPanel);
    }

    [Fact]
    public async Task Create_and_unregister_update_ui_rows_without_deleting_file()
    {
        Directory.CreateDirectory(_directory);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var mapPath = Path.Combine(_directory, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(mapPath));
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        Assert.Single(vm.DatasetItems);
        Assert.Equal("正常", vm.DatasetItems[0].Status);
        var id = vm.DatasetItems[0].Id;
        var datasetPath = Path.Combine(_directory, "data", $"{id}.json");
        vm.SelectDataset(id);
        Assert.True(await vm.UnregisterDatasetAsync());
        Assert.Empty(vm.DatasetItems);
        Assert.True(File.Exists(datasetPath));
    }

    [Fact]
    public async Task Dataset_selection_projects_to_layer_and_inspector()
    {
        Directory.CreateDirectory(_directory);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_directory, "map.json")));
        Assert.True(await vm.CreateDatasetAsync());
        var row = Assert.Single(vm.DatasetItems);
        vm.SelectDataset(row.Id);
        Assert.Equal(row.Id, Assert.Single(vm.DatasetLayerItems, item => item.IsSelected).Id);
        Assert.Equal(row.Name, vm.InspectorSelectionTitle);
        Assert.Contains(vm.InspectorFields, field => field.Label == "数据集 ID" && field.Value == row.Id);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
        catch (IOException) { }
    }
}
