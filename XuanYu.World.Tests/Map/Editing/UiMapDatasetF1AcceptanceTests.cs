using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetF1AcceptanceTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-dataset-f1-{Guid.NewGuid():N}");

    async Task<(UiVm Vm, string Path, string Id)> ReadyAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        return (vm, path, vm.DatasetItems.Single().Id);
    }

    [Fact]
    public async Task Dataset_name_is_editable_and_preserves_identity_and_layer_state()
    {
        var (vm, path, id) = await ReadyAsync();
        await vm.ToggleDatasetVisibilityAsync(id);
        await vm.ToggleDatasetLockAsync(id);
        vm.DatasetNameText = "广东行政区";
        await vm.RenameSelectedDatasetAsync();
        var row = Assert.Single(vm.DatasetItems);
        Assert.Equal("广东行政区", row.Name);
        Assert.Equal(id, row.Id); Assert.False(row.IsVisible); Assert.True(row.IsLocked);
        Assert.True(await vm.SaveMapManifestAsync(path));
        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        Assert.Equal("广东行政区", Assert.Single(reopened.DatasetItems).Name);
    }

    [Fact]
    public void Dataset_ui_rows_stretch_and_drag_visuals_do_not_replace_projection()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        var left = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Right", "DatasetPanel.axaml"));
        var drag = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Right", "DatasetLayerPanel.Drag.cs"));
        Assert.Contains("ListBoxItem", left);
        Assert.Contains("HorizontalContentAlignment\" Value=\"Stretch", left);
        Assert.Contains("DatasetNameText", left);
        Assert.DoesNotContain("SetDatasetLayerDragging", drag);
        Assert.DoesNotContain("SetDatasetLayerDropTarget", drag);
        Assert.DoesNotContain("_datasetItems", drag);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
