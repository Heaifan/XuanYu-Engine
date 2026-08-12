using XuanYu.Editor.UI;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetRegionBootstrapTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-bootstrap-{Guid.NewGuid():N}");

    [Fact]
    public async Task Drawing_request_auto_creates_one_region_dataset()
    {
        var vm = await NewSavedVmAsync();
        EnterRegionWorkspace(vm);
        Assert.Empty(vm.RegionDatasetItems);
        Assert.True(vm.CanRequestRegionDrawing);
        Assert.True(await vm.BeginRegionDrawingAsync());
        Assert.True(vm.IsRegionDrawingTool);
        Assert.Single(vm.RegionDatasetItems);
        Assert.Contains("已自动创建区域数据集", vm.FooterMessage);
        Assert.Equal(MapDatasetLayerIdProjection.Project(vm.DatasetSelectedId!), vm.MapSession.ActiveRegionLayerId);
    }

    [Fact]
    public async Task Double_request_does_not_create_two_datasets()
    {
        var vm = await NewSavedVmAsync();
        EnterRegionWorkspace(vm);
        var first = vm.BeginRegionDrawingAsync();
        var second = vm.BeginRegionDrawingAsync();
        var results = await Task.WhenAll(first, second);
        Assert.Equal(1, results.Count(value => value));
        Assert.Single(vm.RegionDatasetItems);
    }

    [Fact]
    public async Task Locked_region_dataset_is_rejected_without_creation()
    {
        var vm = await NewSavedVmAsync();
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        var id = vm.DatasetSelectedId!;
        await vm.ToggleDatasetLockAsync(id);
        EnterRegionWorkspace(vm);
        Assert.False(await vm.BeginRegionDrawingAsync());
        Assert.Single(vm.RegionDatasetItems);
        Assert.Contains("已锁定", vm.FooterMessage);
    }

    async Task<UiVm> NewSavedVmAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        return vm;
    }

    static void EnterRegionWorkspace(UiVm vm)
    {
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch (IOException) { }
    }
}
