using System.Linq;
using Xunit;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class FeatureEditWorkflowRuntimeTests
{
    [Fact]
    public void Non_feature_edit_mode_disallows_creation()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.False(vm.IsFeatureEditingActive);
        Assert.False(vm.CanRequestMarkerPlacement);
        Assert.False(vm.CanRequestRoadDrawing);
        Assert.False(vm.CanRequestRegionDrawing);
    }

    [Fact]
    public void Enter_feature_edit_mode_sets_state_correctly()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        Assert.True(vm.IsFeatureEditingActive);
        Assert.Equal(EditorWorkspaceId.RegionEditor, vm.CurrentWorkspace.Id);
    }

    [Fact]
    public void Select_feature_does_not_enter_geometry_edit_automatically()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);

        var layerId = vm.MapSession.ActiveRegionLayerId;
        var marker = new MapMarker(MapMarkerId.New(), layerId, "T", new(0, 0));
        var road = new MapRoad(MapRoadId.New(), layerId, "T", "T", [new(0, 0), new(1, 1)]);
        var region = new MapRegion(MapRegionId.New(), layerId, "T", XuanYu.World.Map.MapRegionKind.Generic, [new(0, 0), new(1, 0), new(0, 1)]);
        vm.MapSession.CreateMarker(marker); vm.MapSession.CreateRoad(road); vm.MapSession.CreateRegion(region);
        var selections = new[] {
            new MapGeometrySelection(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()),
            new MapGeometrySelection(MapGeometryFeatureKind.Road, road.RoadId.ToString()),
            new MapGeometrySelection(MapGeometryFeatureKind.Region, region.RegionId.ToString())
        };

        foreach (var selection in selections)
        {
            vm.SelectMapGeometry(selection);
            Assert.False(vm.IsGeometryEditingActive);
            Assert.False(vm.IsMapGeometryDragActive);
            
            vm.ToggleGeometryEditingCommand.Execute(null);
            Assert.True(vm.IsGeometryEditingActive);
            vm.ToggleGeometryEditingCommand.Execute(null); // toggle off for next iteration
        }
    }

    [Fact]
    public void Feature_edit_exit_cleans_state()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        vm.ToggleFeatureEditingCommand.Execute(null);
        Assert.False(vm.IsFeatureEditingActive);
        Assert.Equal(EditorWorkspaceId.MapEditor, vm.CurrentWorkspace.Id);
    }
}
