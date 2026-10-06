using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Scene;

public sealed class BlankSceneContractTests
{
    [Fact]
    public async Task New_scene_rebuilds_legal_map_manifest_registry_and_drawing_journey()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.NewBlankScene();
        AssertBlank(vm);
        await DrawingJourneyAsync(vm);

        var firstMapId = vm.MapSession.CurrentMap.MapId;
        vm.NewBlankScene();
        AssertBlank(vm);
        Assert.NotEqual(firstMapId, vm.MapSession.CurrentMap.MapId);
        await DrawingJourneyAsync(vm);
    }

    [Fact]
    public void P3_probe_add_layer_survives_initial_and_repeated_new_scene()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.AddLayer();
        Assert.Equal(4, vm.MapSession.CurrentMap.Layers.Length);
        vm.NewBlankScene(); vm.AddLayer();
        Assert.Equal(4, vm.MapSession.CurrentMap.Layers.Length);
        vm.NewBlankScene(); vm.NewBlankScene(); vm.AddLayer();
        Assert.Equal(4, vm.MapSession.CurrentMap.Layers.Length);
    }

    static void AssertBlank(UiVm vm)
    {
        Assert.True(MapDefinitionValidator.Validate(vm.MapSession.CurrentMap).Succeeded);
        Assert.True(vm.MapSession.CurrentMap.MapId.IsValid);
        Assert.Equal(vm.MapSession.CurrentMap.MapId.Value, vm.CurrentMapManifest.Id);
        Assert.Empty(vm.DatasetItems);
        Assert.Equal(3, vm.MapSession.CurrentMap.Layers.Length);
        Assert.Single(MapLayerStack.RegionLayers(vm.MapSession.CurrentMap.Layers));
        Assert.True(vm.MapSession.ActiveRegionLayerId.IsValid);
    }

    static async Task DrawingJourneyAsync(UiVm vm)
    {
        if (!vm.IsEditMode) vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.SelectRegionAuthoringMode("地图标记");
        Assert.True(await vm.BeginMarkerPlacementAsync());
        vm.SelectToolCommand.Execute("选择");
        Assert.False(vm.IsMarkerPlacementTool);
        vm.SelectRegionAuthoringMode("道路");
        Assert.True(await vm.BeginRoadDrawingAsync());
        Assert.True(vm.CancelRoadDrawing());
        vm.SelectRegionAuthoringMode("区域面");
        Assert.True(await vm.BeginRegionDrawingAsync());
        Assert.True(vm.CancelRegionDrawing());
    }
}
