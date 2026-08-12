using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Workspace;

public sealed class RegionAuthoringHierarchyTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-r2-f1-{Guid.NewGuid():N}");

    [Fact]
    public void Workspace_definitions_only_expose_map_and_region()
    {
        Assert.Equal([EditorWorkspaceId.MapEditor, EditorWorkspaceId.RegionEditor],
            EditorWorkspaceDefinitions.All.Select(item => item.Id));
    }

    [Fact]
    public void Region_workspace_defaults_to_region_surface_without_selection()
    {
        var vm = CreateVm();
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(vm.IsRegionEditMode);
        Assert.Equal(RegionAuthoringMode.RegionSurface, vm.CurrentRegionAuthoringMode);
        Assert.True(vm.IsRegionSurfaceAuthoringMode);
    }

    [Fact]
    public async Task Selected_road_dataset_enters_region_workspace_in_road_mode()
    {
        var vm = await CreateMappedVmAsync();
        vm.DatasetCreateType = MapDatasetTypes.Road;
        Assert.True(await vm.CreateDatasetAsync());
        var roadId = vm.DatasetItems.Single().Id;
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        Assert.Equal(RegionAuthoringMode.Road, vm.CurrentRegionAuthoringMode);
        Assert.Equal(roadId, vm.DatasetSelectedId);
        Assert.True(vm.IsRoadAuthoringMode);
    }

    [Fact]
    public async Task Selecting_regional_layer_syncs_dataset_and_authoring_mode()
    {
        var vm = await CreateMappedVmAsync();
        vm.DatasetCreateType = MapDatasetTypes.Region;
        Assert.True(await vm.CreateDatasetAsync());
        var regionId = vm.DatasetItems.Single().Id;
        vm.DatasetCreateType = MapDatasetTypes.Road;
        Assert.True(await vm.CreateDatasetAsync());
        var roadId = vm.DatasetItems.Single(item => item.Type == MapDatasetTypes.Road).Id;
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        var road = vm.CurrentLayerItems.Single(row => vm.TryGetDatasetIdForLayer(row.LayerId, out var id) && id == roadId);
        vm.SelectedLayer = road;
        Assert.Equal(roadId, vm.DatasetSelectedId);
        Assert.True(vm.IsRoadAuthoringMode);
        var region = vm.CurrentLayerItems.Single(row => vm.TryGetDatasetIdForLayer(row.LayerId, out var id) && id == regionId);
        vm.SelectedLayer = region;
        Assert.Equal(regionId, vm.DatasetSelectedId);
        Assert.True(vm.IsRegionSurfaceAuthoringMode);
    }

    [Fact]
    public void Switching_authoring_mode_does_not_bootstrap_a_dataset()
    {
        var vm = CreateVm();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        vm.SelectRegionAuthoringModeCommand.Execute("道路");
        Assert.Equal(RegionAuthoringMode.Road, vm.CurrentRegionAuthoringMode);
        Assert.Null(vm.DatasetSelectedId);
        Assert.Empty(vm.DatasetItems);
    }

    UiVm CreateVm() => new(null, () => true, seedInitialScene: false);

    async Task<UiVm> CreateMappedVmAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = CreateVm();
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        return vm;
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
