using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetRegionToolInvalidTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-invalid-{Guid.NewGuid():N}");

    [Fact]
    public async Task Invalid_non_region_dataset_cannot_enable_region_drawing()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        var id = vm.DatasetSelectedId!;
        Assert.True(await vm.SaveMapManifestAsync(path));
        await File.WriteAllTextAsync(Path.Combine(_root, "data", $"{id}.json"), "{}");

        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        reopened.ToggleEditorMode();
        reopened.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        reopened.SelectDataset(id);

        Assert.Equal("无效", reopened.SelectedDataset!.Status);
        Assert.False(reopened.CanStartRegionDrawing);
        reopened.SelectToolCommand.Execute("区域绘制");
        Assert.False(reopened.IsRegionDrawingTool);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
