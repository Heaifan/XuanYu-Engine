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

        foreach (var kind in new[] { MapGeometryFeatureKind.Marker, MapGeometryFeatureKind.Road, MapGeometryFeatureKind.Region })
        {
            vm.SelectMapGeometry(new MapGeometrySelection(kind, "id"));
            Assert.False(vm.IsGeometryEditingActive);
            Assert.False(vm.IsMapGeometryDragActive);
            
            vm.ToggleGeometryEditingCommand.Execute(null);
            Assert.True(vm.IsGeometryEditingActive);
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
