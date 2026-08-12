using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetRegionToolActivationTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-tool-{Guid.NewGuid():N}");

    [Fact]
    public async Task Region_tool_requires_region_edit_mode_and_valid_region_dataset()
    {
        var (vm, id) = await CreateRegionAsync();
        vm.ToggleEditorMode();
        vm.SelectDataset(id);
        vm.SelectToolCommand.Execute("区域绘制");
        Assert.False(vm.IsRegionDrawingTool);
        Assert.Equal("状态：不可用", vm.FooterState);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(vm.CanStartRegionDrawing);
        vm.SelectToolCommand.Execute("区域绘制");
        Assert.True(vm.IsRegionDrawingTool);
    }

    [Fact]
    public async Task Locked_region_and_non_region_datasets_fail_closed()
    {
        var (vm, regionId) = await CreateRegionAsync();
        vm.DatasetCreateType = "road";
        Assert.True(await vm.CreateDatasetAsync());
        var roadId = vm.DatasetSelectedId!;
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        await vm.ToggleDatasetLockAsync(regionId);
        vm.SelectDataset(regionId);
        Assert.False(vm.CanStartRegionDrawing);
        vm.SelectToolCommand.Execute("区域绘制");
        Assert.False(vm.IsRegionDrawingTool);
        vm.SelectDataset(roadId);
        Assert.False(vm.CanStartRegionDrawing);
        vm.SelectToolCommand.Execute("区域绘制");
        Assert.False(vm.IsRegionDrawingTool);
    }

    [Fact]
    public async Task Leaving_region_workspace_cancels_draft_and_tool()
    {
        var (vm, id) = await CreateRegionAsync();
        vm.ToggleEditorMode(); vm.SelectDataset(id);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.SelectToolCommand.Execute("区域绘制");
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var hit = Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
            .Select(y => (X: x * 50.0, Y: y * 50.0))).First(point =>
                MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, point.X, point.Y, out _));
        Assert.True(vm.RegionDrawingPointerPressed(hit.X, hit.Y, viewport));
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.False(vm.IsRegionDrawingTool);
        Assert.False(vm.IsRegionDrawingDraftActive);
    }

    async Task<(UiVm Vm, string Id)> CreateRegionAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        return (vm, vm.DatasetSelectedId!);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
